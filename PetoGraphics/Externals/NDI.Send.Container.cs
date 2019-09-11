using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NAudio.Wave;

namespace NDI
{
    // This file can be built in Expression Blend, but will not function.
    // pInvoke calls can only happen in x86 or x64 platforms.
    // Blend requires an AnyCPU configuration.
    // If used an an AnyCPU configuration, you MUST define ANYCPU in the project properties.
    // It will not function to send video, however it will allow you to design your layout.
    public class NdiSendContainer : Border, INotifyPropertyChanged
    {
        public static readonly DependencyProperty NdiWidthProperty =
            DependencyProperty.Register("NdiWidth", typeof(int), typeof(NdiSendContainer), new PropertyMetadata(1280));

        public static readonly DependencyProperty NdiHeightProperty =
            DependencyProperty.Register("NdiHeight", typeof(int), typeof(NdiSendContainer), new PropertyMetadata(720));

        public static readonly DependencyProperty NdiFrameRateNumeratorProperty =
            DependencyProperty.Register("NdiFrameRateNumerator", typeof(int), typeof(NdiSendContainer), new PropertyMetadata(60000));

        public static readonly DependencyProperty NdiFrameRateDenominatorProperty =
            DependencyProperty.Register("NdiFrameRateDenominator", typeof(int), typeof(NdiSendContainer), new PropertyMetadata(1000));

        public static readonly DependencyProperty NdiNameProperty =
            DependencyProperty.Register("NdiName", typeof(string), typeof(NdiSendContainer), new PropertyMetadata("Unnamed - Fix Me.", OnNdiSenderPropertyChanged));

        public static readonly DependencyProperty NdiGroupsProperty =
            DependencyProperty.Register("NdiGroups", typeof(List<string>), typeof(NdiSendContainer), new PropertyMetadata(new List<string>(), OnNdiSenderPropertyChanged));

        public static readonly DependencyProperty NdiClockToVideoProperty =
            DependencyProperty.Register("NdiClockToVideo", typeof(bool), typeof(NdiSendContainer), new PropertyMetadata(true, OnNdiSenderPropertyChanged));

        public static readonly DependencyProperty NdiClockToAudioProperty =
            DependencyProperty.Register("NdiClockToAudio", typeof(bool), typeof(NdiSendContainer), new PropertyMetadata(false, OnNdiSenderPropertyChanged));

        public static readonly DependencyProperty IsOnProgramProperty =
            DependencyProperty.Register("IsOnProgram", typeof(bool), typeof(NdiSendContainer), new PropertyMetadata(false));

        public static readonly DependencyProperty IsOnPreviewProperty =
            DependencyProperty.Register("IsOnPreview", typeof(bool), typeof(NdiSendContainer), new PropertyMetadata(false));

        private object sendInstanceLock = new object();
        private IntPtr sendInstancePtr = IntPtr.Zero;
        private RenderTargetBitmap targetBitmap;
        private int stride;
        private int videobuffersize;
        private float aspectRatio;
        private WaveIn waveIn;
        private int audiobuffersize;
        private bool sendaudio = false;
        private bool sendvideo = true;

        // a threads to send frames on so that the UI isn't dragged down
        private Thread sendThread;

        // a way to exit the thread safely
        private bool exitThread;

        // a thread safe collection to store pending frames
        private BlockingCollection<NDI.NDIlib_video_frame_t> pendingvideoFrames = new BlockingCollection<NDI.NDIlib_video_frame_t>();

        private BlockingCollection<NDI.NDIlib_audio_frame_interleaved_32f_t> pendingaudioFrames = new BlockingCollection<NDI.NDIlib_audio_frame_interleaved_32f_t>();

        // used for pausing the send thread
        private bool isPausedValue = false;

        ~NdiSendContainer()
        {
            NDIstop();

            // shouldn't be needed, but proper
            Monitor.Enter(sendInstanceLock);

            // Destroy the NDI sender
            if (sendInstancePtr != IntPtr.Zero)
            {
                NDI.Send.NDIlib_send_destroy(sendInstancePtr);

                // Not required, but "correct". (see the SDK documentation)
                NDI.Common.NDIlib_destroy();
            }

            // unlock
            Monitor.Exit(sendInstanceLock);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [Category("NewTek NDI"),
                                                                                                                                                                                                                                Description("NDI output width in pixels. Required.")]
        public int NdiWidth
        {
            get { return (int)GetValue(NdiWidthProperty); }
            set { SetValue(NdiWidthProperty, value); }
        }

        [Category("NewTek NDI"),
        Description("NDI output height in pixels. Required.")]
        public int NdiHeight
        {
            get { return (int)GetValue(NdiHeightProperty); }
            set { SetValue(NdiHeightProperty, value); }
        }

        [Category("NewTek NDI"),
        Description("NDI output frame rate numerator. Required.")]
        public int NdiFrameRateNumerator
        {
            get { return (int)GetValue(NdiFrameRateNumeratorProperty); }
            set { SetValue(NdiFrameRateNumeratorProperty, value); }
        }

        [Category("NewTek NDI"),
        Description("NDI output frame rate denominator. Required.")]
        public int NdiFrameRateDenominator
        {
            get { return (int)GetValue(NdiFrameRateDenominatorProperty); }
            set { SetValue(NdiFrameRateDenominatorProperty, value); }
        }

        [Category("NewTek NDI"),
        Description("NDI output name as displayed to receivers. Required.")]
        public string NdiName
        {
            get { return (string)GetValue(NdiNameProperty); }
            set { SetValue(NdiNameProperty, value); }
        }

        [Category("NewTek NDI"),
        Description("NDI groups this sender will belong to. Optional.")]
        public List<string> NdiGroups
        {
            get { return (List<string>)GetValue(NdiGroupsProperty); }
            set { SetValue(NdiGroupsProperty, value); }
        }

        [Category("NewTek NDI"),
        Description("If clocked to video, NDI will rate limit drawing to the specified frame rate. Defaults to true.")]
        public bool NdiClockToVideo
        {
            get { return (bool)GetValue(NdiClockToVideoProperty); }
            set { SetValue(NdiClockToVideoProperty, value); }
        }

        [Category("NewTek NDI"),
        Description("If clocked to audio, NDI will rate limit playing to the specified frame rate. Defaults to false.")]
        public bool NdiClockToAudio
        {
            get { return (bool)GetValue(NdiClockToAudioProperty); }
            set { SetValue(NdiClockToAudioProperty, value); }
        }

        [Category("NewTek NDI"),
        Description("True if some receiver has this source on program out.")]
        public bool IsOnProgram
        {
            get { return (bool)GetValue(IsOnProgramProperty); }
            set { SetValue(IsOnProgramProperty, value); }
        }

        [Category("NewTek NDI"),
        Description("True if some receiver has this source on preview out.")]
        public bool IsOnPreview
        {
            get { return (bool)GetValue(IsOnPreviewProperty); }
            set { SetValue(IsOnPreviewProperty, value); }
        }

        [Category("NewTek NDI"),
        Description("If True, the send thread does not send, taking no CPU time.")]
        public bool IsSendPaused
        {
            get { return isPausedValue; }
            set
            {
                if (value != isPausedValue)
                {
                    isPausedValue = value;
                    NotifyPropertyChanged("IsSendPaused");
                }
            }
        }

        public bool NDIinit()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                return false;

            // Not required, but "correct". (see the SDK documentation)
            if (!NDI.Common.NDIlib_initialize())
            {
                // Cannot run NDI. Most likely because the CPU is not sufficient (see SDK documentation).
                // you can check this directly with a call to NDIlib_is_supported_CPU()
                PetoGraphics.CustomMessageBox.Show("Cannot run NDI. Most likely because the CPU is not sufficient.");
                return false;
            }

            // Init audio
            if (sendaudio)
                initAudio();

            return true;
        }

        public void NDIstart()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                return;

            exitThread = false;

            pendingvideoFrames = new BlockingCollection<NDI.NDIlib_video_frame_t>();
            pendingaudioFrames = new BlockingCollection<NDI.NDIlib_audio_frame_interleaved_32f_t>();

            sendThread = null;

            // start up a send thread
            sendThread = new Thread(SendThreadProc) { IsBackground = true, Name = "PetoGraphicsNDISendVideo" };
            sendThread.Start();

            if (sendvideo)
            {
                CompositionTarget.Rendering += OnCompositionTargetRendering;
            }

            if (sendaudio)
            {
                waveIn.StartRecording();
            }
        }

        public void NDIstop()
        {
            if (waveIn != null)
                waveIn.StopRecording();

            // tell the thread to exit
            exitThread = true;

            CompositionTarget.Rendering -= OnCompositionTargetRendering;

            // wait for it to exit
            if (sendThread != null)
                sendThread.Join();

            // cause the pulling of frames to fail
            pendingaudioFrames.CompleteAdding();

            // clear any pending frames
            while (pendingaudioFrames.Count > 0)
            {
                NDI.NDIlib_audio_frame_interleaved_32f_t discardFrame = pendingaudioFrames.Take();
                Marshal.FreeHGlobal(discardFrame.p_data);
            }

            // cause the pulling of frames to fail
            pendingvideoFrames.CompleteAdding();

            // clear any pending frames
            while (pendingvideoFrames.Count > 0)
            {
                NDI.NDIlib_video_frame_t discardFrame = pendingvideoFrames.Take();
                Marshal.FreeHGlobal(discardFrame.p_data);
            }
        }

        public void SendVideoFrame()
        {
            // get the next available frame
            NDI.NDIlib_video_frame_t videoframe;
            if (pendingvideoFrames.TryTake(out videoframe))
            {
                // this drops frames if the UI is rendering ahead of the specified NDI frame rate
                while (pendingvideoFrames.Count > 1)
                {
                    NDI.NDIlib_video_frame_t discardFrame = pendingvideoFrames.Take();
                    Marshal.FreeHGlobal(discardFrame.p_data);
                }

                // We now submit the frame. Note that this call will be clocked so that we end up submitting 
                // at exactly the requested rate.
                NDI.Send.NDIlib_send_send_video(sendInstancePtr, ref videoframe);

                // free the memory from this frame
                Marshal.FreeHGlobal(videoframe.p_data);
            }
        }

        // the receive thread runs though this loop until told to exit
        public void SendAudioFrame()
        {
            // get the next available frame
            NDI.NDIlib_audio_frame_interleaved_32f_t audioframe;
            if (pendingaudioFrames.TryTake(out audioframe))
            {
                // this drops frames if the UI is rendering ahead of the specified NDI frame rate
                while (pendingaudioFrames.Count > 1)
                {
                    NDI.NDIlib_audio_frame_interleaved_32f_t discardFrame = pendingaudioFrames.Take();
                    Marshal.FreeHGlobal(discardFrame.p_data);
                }
                // We now submit the frame. Note that this call will be clocked so that we end up submitting 
                // at exactly the requested rate.
                NDI.Utilities.NDIlib_util_send_send_audio_interleaved_32f(sendInstancePtr, ref audioframe);

                // free the memory from this frame
                Marshal.FreeHGlobal(audioframe.p_data);
            }
        }

        public bool AddVideoFrame(NDI.NDIlib_video_frame_t frame)
        {
            try
            {
                pendingvideoFrames.Add(frame);
            }
            catch (OperationCanceledException)
            {
                // we're shutting down
                pendingvideoFrames.CompleteAdding();
                return false;
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool AddAudioFrame(NDI.NDIlib_audio_frame_interleaved_32f_t frame)
        {
            try
            {
                pendingaudioFrames.Add(frame);
            }
            catch (OperationCanceledException)
            {
                // we're shutting down
                pendingaudioFrames.CompleteAdding();
                return false;
            }
            catch
            {
                return false;
            }

            return true;
        }

        private static void OnNdiSenderPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            NdiSendContainer s = sender as NdiSendContainer;
            if (s != null)
                s.InitializeNdi();
        }

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
        private void initAudio()
        {
            waveIn = new WaveIn();
            waveIn.WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);
            waveIn.DeviceNumber = 0;
            waveIn.BufferMilliseconds = 100;
            waveIn.DataAvailable += new EventHandler<WaveInEventArgs>(wavein_DataAvailable);
        }
        private void OnCompositionTargetRendering(object sender, EventArgs e)
        {
            RenderFrame();
        }

        private void RenderFrame()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            int xres = NdiWidth;
            int yres = NdiHeight;

            int frNum = NdiFrameRateNumerator;
            int frDen = NdiFrameRateDenominator;

            // sanity
            if (sendInstancePtr == IntPtr.Zero || xres < 8 || yres < 8)
                return;

            if (targetBitmap == null || targetBitmap.PixelWidth != xres || targetBitmap.PixelHeight != yres)
            {
                // Create a properly sized RenderTargetBitmap
                targetBitmap = new RenderTargetBitmap(xres, yres, 96, 96, PixelFormats.Pbgra32);
            }

            // clear to prevent trails
            targetBitmap.Clear();

            // render the content into the bitmap
            targetBitmap.Render(Child);

            stride = (xres * 32/*BGRA bpp*/ + 7) / 8;
            videobuffersize = yres * stride;
            aspectRatio = xres / (float)yres;

            // allocate some memory for a video buffer
            IntPtr bufferPtr = Marshal.AllocHGlobal(videobuffersize);

            // We are going to create a progressive frame at 60Hz.
            NDIlib_video_frame_t videoFrame = new NDI.NDIlib_video_frame_t()
            {
                // Resolution
                xres = (uint)NdiWidth,
                yres = (uint)NdiHeight,
                // Use BGRA video
                FourCC = NDI.NDIlib_FourCC_type_e.NDIlib_FourCC_type_BGRA,
                // The frame-rate
                frame_rate_N = (uint)frNum,
                frame_rate_D = (uint)frDen,
                // The aspect ratio
                picture_aspect_ratio = aspectRatio,
                // This is a progressive frame
                frame_format_type = NDIlib_frame_format_type_e.NDIlib_frame_format_type_progressive,
                // Timecode.
                timecode = NDI.Common.NDIlib_send_timecode_synthesize,
                // The video memory used for this frame
                p_data = bufferPtr,
                // The line to line stride of this image
                line_stride_in_bytes = (uint)stride
            };

            // copy the pixels into the buffer
            targetBitmap.CopyPixels(new Int32Rect(0, 0, xres, yres), bufferPtr, videobuffersize, stride);

            // add it to the output queue
            AddVideoFrame(videoFrame);
        }

        private void wavein_DataAvailable(object sender, WaveInEventArgs e)
        {
            audiobuffersize = e.BytesRecorded * sizeof(float);
            IntPtr audiobufferPtr = Marshal.AllocHGlobal(audiobuffersize);

            NDIlib_audio_frame_interleaved_32f_t audioFrame = new NDI.NDIlib_audio_frame_interleaved_32f_t()
            {
                // 48kHz
                sample_rate = (uint)waveIn.WaveFormat.SampleRate,
                // Submit stereo
                no_channels = (uint)waveIn.WaveFormat.Channels,
                // There can be up to 1602 samples
                no_samples = (uint)e.BytesRecorded / (uint)waveIn.WaveFormat.Channels,
                // Timecode (synthesized for us !).
                timecode = NDI.Common.NDIlib_send_timecode_synthesize,
                // The buffer
                p_data = audiobufferPtr,
            };

            WaveBuffer wavebuffer = new WaveBuffer(e.Buffer);
            Marshal.Copy(wavebuffer.FloatBuffer, 0, audiobufferPtr, (int)audioFrame.no_samples * (int)audioFrame.no_channels);

            AddAudioFrame(audioFrame);
        }
        private void InitializeNdi()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            Monitor.Enter(sendInstanceLock);


            // we need a name
            if (string.IsNullOrEmpty(NdiName))
                return;

            // re-initialize?
            if (sendInstancePtr != IntPtr.Zero)
            {
                NDI.Send.NDIlib_send_destroy(sendInstancePtr);
                sendInstancePtr = IntPtr.Zero;
            }

            // .Net interop doesn't handle UTF-8 strings, so do it manually
            // These must be freed later
            IntPtr sourceNamePtr = Common.StringToUtf8(NdiName);

            IntPtr groupsNamePtr = IntPtr.Zero;

            // build a comma separated list of groups?
            if (NdiGroups.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < NdiGroups.Count(); i++)
                {
                    sb.Append(NdiGroups[i]);

                    if (i < NdiGroups.Count - 1)
                        sb.Append(',');
                }

                groupsNamePtr = Common.StringToUtf8(sb.ToString());
            }

            // Create an NDI source description using sourceNamePtr and it's clocked to the video.
            NDIlib_send_create_t createDesc = new NDIlib_send_create_t()
            {
                p_ndi_name = sourceNamePtr,
                p_groups = groupsNamePtr,
                clock_video = NdiClockToVideo,
                clock_audio = NdiClockToAudio
            };

            // We create the NDI finder instance
            sendInstancePtr = Send.NDIlib_send_create(ref createDesc);

            // free the strings we allocated
            Marshal.FreeHGlobal(sourceNamePtr);
            Marshal.FreeHGlobal(groupsNamePtr);

            // unlock
            Monitor.Exit(sendInstanceLock);
        }
        
        // the send thread runs though this loop until told to exit
        private void SendThreadProc()
        {
            // look for changes in tally
            bool lastProg = false;
            bool lastPrev = false;

            NDIlib_tally_t tally = new NDIlib_tally_t();
            tally.on_program = lastProg;
            tally.on_preview = lastPrev;

            while (!exitThread)
            {
                if (!IsSendPaused && Monitor.TryEnter(sendInstanceLock))
                {
                    // if this is not here, then we must be being reconfigured
                    if (sendInstancePtr == null)
                    {
                        // unlock
                        Monitor.Exit(sendInstanceLock);
                        
                        // give up some time
                        Thread.Sleep(20);
                        
                        // loop again
                        continue;
                    }

                    try
                    {
                        if (sendaudio)
                        {
                            SendAudioFrame();
                        }
                        if (sendvideo)
                        {
                            SendVideoFrame();
                        }

                    }
                    catch (OperationCanceledException)
                    {
                        pendingvideoFrames.CompleteAdding();
                        pendingaudioFrames.CompleteAdding();
                    }
                    catch
                    {
                    }

                    // unlock
                    Monitor.Exit(sendInstanceLock);
                }
                else
                {
                    Thread.Sleep(20);
                }

                // check tally
                Send.NDIlib_send_get_tally(sendInstancePtr, ref tally, 0);

                // if tally changed trigger an update
                if (lastProg != tally.on_program || lastPrev != tally.on_preview)
                {
                    // save the last values
                    lastProg = tally.on_program;
                    lastPrev = tally.on_preview;

                    // set these on the UI thread
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        IsOnProgram = lastProg;
                        IsOnPreview = lastPrev;
                    }));
                }
            }
        }
    }
}
