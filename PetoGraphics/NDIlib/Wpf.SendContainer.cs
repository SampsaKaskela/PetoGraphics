using NAudio.CoreAudioApi;
using NAudio.Wave;
using PetoGraphics;
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

namespace NewTek.NDI.WPF
{
    public class NdiSendContainer : Border, INotifyPropertyChanged, IDisposable
    {
        [Category("NewTek NDI"),
        Description("NDI output width in pixels. Required.")]
        public int NdiWidth
        {
            get { return (int)GetValue(NdiWidthProperty); }
            set { SetValue(NdiWidthProperty, value); }
        }
        public static readonly DependencyProperty NdiWidthProperty =
            DependencyProperty.Register("NdiWidth", typeof(int), typeof(NdiSendContainer), new PropertyMetadata(1280));

        [Category("NewTek NDI"),
        Description("NDI output height in pixels. Required.")]
        public int NdiHeight
        {
            get { return (int)GetValue(NdiHeightProperty); }
            set { SetValue(NdiHeightProperty, value); }
        }
        public static readonly DependencyProperty NdiHeightProperty =
            DependencyProperty.Register("NdiHeight", typeof(int), typeof(NdiSendContainer), new PropertyMetadata(720));


        [Category("NewTek NDI"),
        Description("NDI output frame rate numerator. Required.")]
        public int NdiFrameRateNumerator
        {
            get { return (int)GetValue(NdiFrameRateNumeratorProperty); }
            set { SetValue(NdiFrameRateNumeratorProperty, value); }
        }
        public static readonly DependencyProperty NdiFrameRateNumeratorProperty =
            DependencyProperty.Register("NdiFrameRateNumerator", typeof(int), typeof(NdiSendContainer), new PropertyMetadata(60000));
        
        [Category("NewTek NDI"),
        Description("NDI output frame rate denominator. Required.")]
        public int NdiFrameRateDenominator
        {
            get { return (int)GetValue(NdiFrameRateDenominatorProperty); }
            set { SetValue(NdiFrameRateDenominatorProperty, value); }
        }
        public static readonly DependencyProperty NdiFrameRateDenominatorProperty =
            DependencyProperty.Register("NdiFrameRateDenominator", typeof(int), typeof(NdiSendContainer), new PropertyMetadata(1000));


        [Category("NewTek NDI"),
        Description("NDI output name as displayed to receivers. Required.")]
        public String NdiName
        {
            get { return (String)GetValue(NdiNameProperty); }
            set { SetValue(NdiNameProperty, value); }
        }
        public static readonly DependencyProperty NdiNameProperty =
            DependencyProperty.Register("NdiName", typeof(String), typeof(NdiSendContainer), new PropertyMetadata("Unnamed - Fix Me.", OnNdiSenderPropertyChanged));


        [Category("NewTek NDI"),
        Description("NDI groups this sender will belong to. Optional.")]
        public List<String> NdiGroups
        {
            get { return (List<String>)GetValue(NdiGroupsProperty); }
            set { SetValue(NdiGroupsProperty, value); }
        }
        public static readonly DependencyProperty NdiGroupsProperty =
            DependencyProperty.Register("NdiGroups", typeof(List<String>), typeof(NdiSendContainer), new PropertyMetadata(new List<String>(), OnNdiSenderPropertyChanged));


        [Category("NewTek NDI"),
        Description("If clocked to video, NDI will rate limit drawing to the specified frame rate. Defaults to true.")]
        public bool NdiClockToVideo
        {
            get { return (bool)GetValue(NdiClockToVideoProperty); }
            set { SetValue(NdiClockToVideoProperty, value); }
        }
        public static readonly DependencyProperty NdiClockToVideoProperty =
            DependencyProperty.Register("NdiClockToVideo", typeof(bool), typeof(NdiSendContainer), new PropertyMetadata(true, OnNdiSenderPropertyChanged));

        [Category("NewTek NDI"),
        Description("True if some receiver has this source on program out.")]
        public bool IsOnProgram
        {
            get { return (bool)GetValue(IsOnProgramProperty); }
            set { SetValue(IsOnProgramProperty, value); }
        }
        public static readonly DependencyProperty IsOnProgramProperty =
            DependencyProperty.Register("IsOnProgram", typeof(bool), typeof(NdiSendContainer), new PropertyMetadata(false));

        [Category("NewTek NDI"),
        Description("True if some receiver has this source on preview out.")]
        public bool IsOnPreview
        {
            get { return (bool)GetValue(IsOnPreviewProperty); }
            set { SetValue(IsOnPreviewProperty, value); }
        }
        public static readonly DependencyProperty IsOnPreviewProperty =
            DependencyProperty.Register("IsOnPreview", typeof(bool), typeof(NdiSendContainer), new PropertyMetadata(false));


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

        [Category("NewTek NDI"),
        Description("If you need partial transparency, set this to true. If not, set to false and save some CPU cycles.")]
        public bool UnPremultiply
        {
            get { return unPremultiply; }
            set
            {
                if (value != unPremultiply)
                {
                    unPremultiply = value;
                    NotifyPropertyChanged("UnPremultiply");
                }
            }
        }

        [Category("NewTek NDI"),
        Description("If true audio will be send.")]
        public bool IsAudioEnabled
        {
            get { return audioEnabled; }
            set
            {
                if (value != audioEnabled)
                {
                    audioEnabled = value;
                    NotifyPropertyChanged("IsAudioEnabled");
                }
            }
        }

        [Category("NewTek NDI"),
        Description("Audio device to listen.")]
        public MMDevice AudioDevice
        {
            get { return audioDevice; }
            set
            {
                if (value != audioDevice)
                {
                    audioDevice = value;
                    NotifyPropertyChanged("AudioDevice");
                }
            }
        }

        public bool Init()
        {
            // Not required, but "correct". (see the SDK documentation)
            if (!NDIlib.initialize())
            {
                // Cannot run NDI. Most likely because the CPU is not sufficient (see SDK documentation).
                // you can check this directly with a call to NDIlib.is_supported_CPU()
                CustomMessageBox.Show("Cannot run NDI");
                return false;
            }
            return true;
        }

        public void Start()
        {
            exitThread = false;

            sendThread = new Thread(SendThreadProc) { IsBackground = true, Name = "PetoGraphicsNdi" };
            sendThread.Start();

            // Video
            CompositionTarget.Rendering += OnCompositionTargetRendering;

            // Audio
            if (audioEnabled)
            {
                if (audioDevice != null)
                {
                    audioCapture = new WasapiLoopbackCapture(audioDevice);
                    audioCapture.DataAvailable += OnDataAvailable;
                    audioCapture.StartRecording();
                }
                else
                {
                    CustomMessageBox.Show("No audio device selected.");
                }
            }
        }

        public void Stop()
        {
            // tell the thread to exit
            exitThread = true;

            // Video
            CompositionTarget.Rendering -= OnCompositionTargetRendering;

            // Audio
            if (audioCapture != null)
            {
                audioCapture.StopRecording();
                audioCapture = null;
            }

            // wait for it to exit
            if (sendThread != null)
            {
                sendThread.Join();

                sendThread = null;
            }

            // cause the pulling of frames to fail
            pendingVideoFrames.CompleteAdding();
            pendingAudioFrames.CompleteAdding();

            // clear any pending video frames
            while (pendingVideoFrames.Count > 0)
            {
                NDIlib.video_frame_v2_t discardFrame = pendingVideoFrames.Take();
                Marshal.FreeHGlobal(discardFrame.p_data);
            }

            // clear any pending audio frames
            while (pendingAudioFrames.Count > 0)
            {
                NDIlib.audio_frame_v2_t discardFrame = pendingAudioFrames.Take();
                Marshal.FreeHGlobal(discardFrame.p_data);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~NdiSendContainer() 
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Stop();
                    pendingVideoFrames.Dispose();
                }
                
                // Destroy the NDI sender
                if (sendInstancePtr != IntPtr.Zero)
                {
                    NDIlib.send_destroy(sendInstancePtr);

                    sendInstancePtr = IntPtr.Zero;
                }

                // Not required, but "correct". (see the SDK documentation)
                NDIlib.destroy();

                _disposed = true;
            }
        }

        private bool _disposed = false;
        
        private void OnCompositionTargetRendering(object sender, EventArgs e)
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                return;

            int xres = NdiWidth;
            int yres = NdiHeight;

            int frNum = NdiFrameRateNumerator;
            int frDen = NdiFrameRateDenominator;

            RenderTargetBitmap targetBitmap = null;
            FormatConvertedBitmap fmtConvertedBmp = null;

            // sanity
            if (sendInstancePtr == IntPtr.Zero || xres < 8 || yres < 8)
                return;

            if (targetBitmap == null || targetBitmap.PixelWidth != xres || targetBitmap.PixelHeight != yres)
            {
                // Create a properly sized RenderTargetBitmap
                targetBitmap = new RenderTargetBitmap(xres, yres, 96, 96, PixelFormats.Pbgra32);

                fmtConvertedBmp = new FormatConvertedBitmap();
                fmtConvertedBmp.BeginInit();
                fmtConvertedBmp.Source = targetBitmap;
                fmtConvertedBmp.DestinationFormat = PixelFormats.Bgra32;
                fmtConvertedBmp.EndInit();
            }

            // clear to prevent trails
            targetBitmap.Clear();

            // render the content into the bitmap
            targetBitmap.Render(this.Child);

            int stride = (xres * 32/*BGRA bpp*/ + 7) / 8;
            int bufferSize = yres * stride;
            float aspectRatio = (float)xres / (float)yres;
            
            // allocate some memory for a video buffer
            IntPtr bufferPtr = Marshal.AllocHGlobal(bufferSize);

            // We are going to create a progressive frame at 60Hz.
            NDIlib.video_frame_v2_t videoFrame = new NDIlib.video_frame_v2_t()
            {
                // Resolution
                xres = NdiWidth,
                yres = NdiHeight,
                // Use BGRA video
                FourCC = NDIlib.FourCC_type_e.FourCC_type_BGRA,
                // The frame-eate
                frame_rate_N = frNum,
                frame_rate_D = frDen,
                // The aspect ratio
                picture_aspect_ratio = aspectRatio,
                // This is a progressive frame
                frame_format_type = NDIlib.frame_format_type_e.frame_format_type_progressive,
                // Timecode.
                timecode = NDIlib.send_timecode_synthesize,
                // The video memory used for this frame
                p_data = bufferPtr,
                // The line to line stride of this image
                line_stride_in_bytes = stride,
                // no metadata
                p_metadata = IntPtr.Zero,
                // only valid on received frames
                timestamp = 0
            };

            if (UnPremultiply && fmtConvertedBmp != null)
            {
                fmtConvertedBmp.CopyPixels(new Int32Rect(0, 0, xres, yres), bufferPtr, bufferSize, stride);
            }
            else
            {
                // copy the pixels into the buffer
                targetBitmap.CopyPixels(new Int32Rect(0, 0, xres, yres), bufferPtr, bufferSize, stride);
            }

            // add it to the output queue
            AddVideoFrame(videoFrame);
        }

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            int numberOfChannels = audioCapture.WaveFormat.Channels;
            int sampleRate = audioCapture.WaveFormat.SampleRate;
            // interpret as 32 bit floating point audio
            int samples = e.BytesRecorded / 4;

            WaveBuffer waveBuffer = new WaveBuffer(e.Buffer);
            float[] buffer = waveBuffer.FloatBuffer;
            for (int index = 0; index < samples; index++)
            {
                buffer[index] = waveBuffer.FloatBuffer[index];
            }

            // allocate some memory for a audio buffer
            IntPtr bufferPtr = Marshal.AllocHGlobal(numberOfChannels * samples * sizeof(float));

            NDIlib.audio_frame_v2_t audioFrame = new NDIlib.audio_frame_v2_t()
            {
                // Sample rate
                sample_rate = sampleRate,
                // Number of channels (1 = mono, 2 = stereo)
                no_channels = numberOfChannels,
                // Number of samples
                no_samples = samples,
                // Timecode.
                timecode = NDIlib.send_timecode_synthesize,
                // The audio memory used for this frame
                p_data = bufferPtr,
                // The inter channel stride
                channel_stride_in_bytes = sizeof(float) * samples,
                // no metadata
                p_metadata = IntPtr.Zero,
                // only valid on received frames
                timestamp = 0
            };

            for (int ch = 0; ch < audioFrame.no_channels; ch++)
            {
                // where does this channel start in the buffer?
                IntPtr dest = new IntPtr(audioFrame.p_data.ToInt64() + (ch * audioFrame.channel_stride_in_bytes));
                // copy the buffer into the channel
                Marshal.Copy(buffer, 0, dest, audioFrame.no_samples);
            }

            // add it to the output queue
            AddAudioFrame(audioFrame);
        }


        private static void OnNdiSenderPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            NdiSendContainer s = sender as NdiSendContainer;
            if (s != null)
                s.InitializeNdi();
        }
        
        private void InitializeNdi()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                return;

            Monitor.Enter(sendInstanceLock);

            // we need a name
            if (String.IsNullOrEmpty(NdiName))
                return;

            // re-initialize?
            if (sendInstancePtr != IntPtr.Zero)
            {
                NDIlib.send_destroy(sendInstancePtr);
                sendInstancePtr = IntPtr.Zero;
            }

            // .Net interop doesn't handle UTF-8 strings, so do it manually
            // These must be freed later
            IntPtr sourceNamePtr = UTF.StringToUtf8(NdiName);

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

                groupsNamePtr = UTF.StringToUtf8(sb.ToString());
            }

            // Create an NDI source description using sourceNamePtr and it's clocked to the video.
            NDIlib.send_create_t createDesc = new NDIlib.send_create_t()
            {
                p_ndi_name = sourceNamePtr,
                p_groups = groupsNamePtr,
                clock_video = NdiClockToVideo,
                clock_audio = false
            };

            // We create the NDI finder instance
            sendInstancePtr = NDIlib.send_create(ref createDesc);

            // free the strings we allocated
            Marshal.FreeHGlobal(sourceNamePtr);
            Marshal.FreeHGlobal(groupsNamePtr);

            // unlock
            Monitor.Exit(sendInstanceLock);
        }
        
        // the receive thread runs though this loop until told to exit
        private void SendThreadProc()
        {
            // look for changes in tally
            bool lastProg = false;
            bool lastPrev = false;

            NDIlib.tally_t tally = new NDIlib.tally_t
            {
                on_program = lastProg,
                on_preview = lastPrev
            };

            while (!exitThread)
            {
                if (Monitor.TryEnter(sendInstanceLock))
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

                    // Audio should be send first
                    if (audioEnabled)
                    {
                        try
                        {
                            // get the next available frame
                            if (pendingAudioFrames.TryTake(out NDIlib.audio_frame_v2_t frame, 250))
                            {
                                // Submit the audio buffer
                                if (!IsSendPaused)
                                {
                                    NDIlib.send_send_audio_v2(sendInstancePtr, ref frame);
                                }

                                // free the memory from this frame
                                Marshal.FreeHGlobal(frame.p_data);
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            pendingAudioFrames.CompleteAdding();
                        }
                        catch {}
                    }

                    try
                    {
                        // get the next available frame
                        if (pendingVideoFrames.TryTake(out NDIlib.video_frame_v2_t frame, 250))
                        {
                            // this drops frames if the UI is rendering ahead of the specified NDI frame rate
                            while (pendingVideoFrames.Count > 1)
                            {
                                NDIlib.video_frame_v2_t discardFrame = pendingVideoFrames.Take();
                                Marshal.FreeHGlobal(discardFrame.p_data);
                            }

                            // We now submit the frame. Note that this call will be clocked so that we end up submitting 
                            // at exactly the requested rate.
                            // If WPF can't keep up with what you requested of NDI, then it will be sent at the rate WPF is rendering.
                            if (!IsSendPaused)
                            {
                                NDIlib.send_send_video_v2(sendInstancePtr, ref frame);
                            }

                            // free the memory from this frame
                            Marshal.FreeHGlobal(frame.p_data);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        pendingVideoFrames.CompleteAdding();
                    }
                    catch {}

                    // unlock
                    Monitor.Exit(sendInstanceLock);
                }
                else
                {
                    Thread.Sleep(20);
                }

                // check tally
                NDIlib.send_get_tally(sendInstancePtr, ref tally, 0);

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

        public bool AddVideoFrame(NDIlib.video_frame_v2_t frame)
        {
            try
            {
                pendingVideoFrames.Add(frame);
            }
            catch (OperationCanceledException)
            {
                // we're shutting down
                pendingVideoFrames.CompleteAdding();
                return false;
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool AddAudioFrame(NDIlib.audio_frame_v2_t frame)
        {
            try
            {
                pendingAudioFrames.Add(frame);
            }
            catch (OperationCanceledException)
            {
                // we're shutting down
                pendingAudioFrames.CompleteAdding();
                return false;
            }
            catch
            {
                return false;
            }

            return true;
        }

        private Object sendInstanceLock = new Object();
        private IntPtr sendInstancePtr = IntPtr.Zero;

        // a way to exit the thread safely
        private bool exitThread = false;

        // used for pausing the send thread
        private bool isPausedValue = false;

        // a safe value at the expense of CPU cycles
        private bool unPremultiply = true;

        // a thread to send frames on so that the UI isn't dragged down
        private Thread sendThread = null;

        // a thread safe collection to store pending frames
        private BlockingCollection<NDIlib.video_frame_v2_t> pendingVideoFrames = new BlockingCollection<NDIlib.video_frame_v2_t>();
        private BlockingCollection<NDIlib.audio_frame_v2_t> pendingAudioFrames = new BlockingCollection<NDIlib.audio_frame_v2_t>();

        // determines if audio should be send
        private bool audioEnabled = false;

        // determines if audio should be send
        private MMDevice audioDevice = null;

        // instance used to capture target audio device
        private WasapiLoopbackCapture audioCapture = null;
    }
}
