using System;
using System.Runtime.InteropServices;
using System.Security;

namespace NDI
{
    // The creation structure that is used when you are creating a sender
    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct NDIlib_send_create_t
    {
        // The name of the NDI source to create. This is a NULL terminated UTF-8 string.
        public IntPtr p_ndi_name;

        // What groups should this source be part of. UTF-8 string
        public IntPtr p_groups;

        // Do you want audio and video to "clock" themselves. When they are clocked then 
        // by adding video frames, they will be rate limited to match the current frame-rate
        // that you are submitting at. The same is true for audio. In general if you are submitting
        // video and audio off a single thread then you should only clock one of them (video is
        // probably the better of the two to clock off). If you are submtiting audio and video
        // of separate threads then having both clocked can be useful.
        [MarshalAsAttribute(UnmanagedType.U1)]
        public bool clock_video;

        [MarshalAsAttribute(UnmanagedType.U1)]
        public bool clock_audio;
    }

    [SuppressUnmanagedCodeSecurity]
    public static class Send
    {
        // Create a new sender instance. This will return null if it fails.
        public static IntPtr NDIlib_send_create(ref NDIlib_send_create_t p_create_settings)
        {
            if (IntPtr.Size == 8)
                return NDIlib64_send_create(ref p_create_settings);
            else
                return NDIlib32_send_create(ref p_create_settings);
        }

        // This will destroy an existing sender instance.
        public static void NDIlib_send_destroy(IntPtr p_instance)
        {
            if (IntPtr.Size == 8)
                NDIlib64_send_destroy(p_instance);
            else
                NDIlib32_send_destroy(p_instance);
        }

        // This will add a video frame
        public static void NDIlib_send_send_video(IntPtr p_instance, ref NDIlib_video_frame_t p_video_data)
        {
            if (IntPtr.Size == 8)
                NDIlib64_send_send_video(p_instance, ref p_video_data);
            else
                NDIlib32_send_send_video(p_instance, ref p_video_data);
        }

        // This will add a video frame and will return immediately, having scheduled the frame to be displayed. 
        // All processing and sending of the video will occur asynchronously. The memory accessed by NDIlib_video_frame_t 
        // cannot be freed or re-used by the caller until a synchronizing event has occured. In general the API is better
        // able to take advantage of asynchronous processing than you might be able to by simple having a separate thread
        // to submit frames. 
        //
        // This call is particularly beneficial when processing BGRA video since it allows any color conversion, compression
        // and network sending to all be done on separate threads from your main rendering thread. 
        //
        // Synchronozing events are :
        //		- a call to NDIlib_send_send_video
        //		- a call to NDIlib_send_send_video_async with another frame to be sent
        //		- a call to NDIlib_send_send_video with p_video_data=NULL
        //		- a call to NDIlib_send_destroy
        public static void NDIlib_send_send_video_async(IntPtr p_instance, ref NDIlib_video_frame_t p_video_data)
        {
            if (IntPtr.Size == 8)
                NDIlib64_send_send_video_async(p_instance, ref p_video_data);
            else
                NDIlib32_send_send_video_async(p_instance, ref p_video_data);
        }

        // This will add an audio frame
        public static void NDIlib_send_send_audio(IntPtr p_instance, ref NDIlib_audio_frame_t p_audio_data)
        {
            if (IntPtr.Size == 8)
                NDIlib64_send_send_audio(p_instance, ref p_audio_data);
            else
                NDIlib32_send_send_audio(p_instance, ref p_audio_data);
        }

        // This will add a meta-data frame
        public static void NDIlib_send_send_metadata(IntPtr p_instance, ref NDIlib_metadata_frame_t p_meta_data)
        {
            if (IntPtr.Size == 8)
                NDIlib64_send_send_metadata(p_instance, ref p_meta_data);
            else
                NDIlib32_send_send_metadata(p_instance, ref p_meta_data);
        }

        // This allows you to receive meta data from the other end of the connection
        public static NDIlib_frame_type_e NDIlib_send_capture(IntPtr p_instance, ref NDIlib_metadata_frame_t p_meta_data, uint timeout_in_ms)
        {
            if (IntPtr.Size == 8)
                return NDIlib64_send_capture(p_instance, ref p_meta_data, timeout_in_ms);
            else
                return NDIlib32_send_capture(p_instance, ref p_meta_data, timeout_in_ms);
        }

        // Free the buffers returned by capture for meta-data
        public static void NDIlib_send_free_metadata(IntPtr p_instance, ref NDIlib_metadata_frame_t p_meta_data)
        {
            if (IntPtr.Size == 8)
                NDIlib64_send_free_metadata(p_instance, ref p_meta_data);
            else
                NDIlib32_send_free_metadata(p_instance, ref p_meta_data);
        }

        // Determine the current tally sate. If you specify a timeout then it will wait until it has changed, otherwise it will simply poll it
        // and return the current tally immediately. The return value is whether anything has actually change (TRUE) or whether it timed out (FALSE)
        public static bool NDIlib_send_get_tally(IntPtr p_instance, ref NDIlib_tally_t p_tally, uint timeout_in_ms)
        {
            if (IntPtr.Size == 8)
                return NDIlib64_send_get_tally(p_instance, ref p_tally, timeout_in_ms);
            else
                return NDIlib32_send_get_tally(p_instance, ref p_tally, timeout_in_ms);
        }

        // Get the current number of receivers connected to this source. This can be used to avoid even rendering when nothing is connected to the video source. 
        // which can significantly improve the efficiency if you want to make a lot of sources available on the network. If you specify a timeout that is not
        // 0 then it will wait until there are connections for this ammount of time.
        public static uint NDIlib_send_get_no_connections(IntPtr p_instance, uint timeout_in_ms)
        {
            if (IntPtr.Size == 8)
                return NDIlib64_send_get_no_connections(p_instance, timeout_in_ms);
            else
                return NDIlib32_send_get_no_connections(p_instance, timeout_in_ms);
        }

        // Connection based metadata is data that is sent automatically each time a new connection is received. You queue all of these
        // up and they are sent on each connection. To reset them you need to clear them all and set them up again. 
        public static void NDIlib_send_clear_connection_metadata(IntPtr p_instance)
        {
            if (IntPtr.Size == 8)
                NDIlib64_send_clear_connection_metadata(p_instance);
            else
                NDIlib32_send_clear_connection_metadata(p_instance);
        }

        // Add a connection metadata string to the list of what is sent on each new connection. If someone is already connected then
        // this string will be sent to them immediately.
        public static void NDIlib_send_add_connection_metadata(IntPtr p_instance, ref NDIlib_metadata_frame_t p_meta_data)
        {
            if (IntPtr.Size == 8)
                NDIlib64_send_add_connection_metadata(p_instance, ref p_meta_data);
            else
                NDIlib32_send_add_connection_metadata(p_instance, ref p_meta_data);
        }

#region pInvoke
        const string NDILib64Name = "Processing.NDI.Lib.x64.dll";
        const string NDILib32Name = "Processing.NDI.Lib.x86.dll";

        [DllImportAttribute(NDILib64Name, EntryPoint = "NDIlib_send_create", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr NDIlib64_send_create(ref NDIlib_send_create_t p_create_settings);
        [DllImportAttribute(NDILib32Name, EntryPoint = "NDIlib_send_create", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr NDIlib32_send_create(ref NDIlib_send_create_t p_create_settings);
        
        [DllImportAttribute(NDILib64Name, EntryPoint = "NDIlib_send_destroy", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void NDIlib64_send_destroy(IntPtr p_instance);
        [DllImportAttribute(NDILib32Name, EntryPoint = "NDIlib_send_destroy", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void NDIlib32_send_destroy(IntPtr p_instance);
        
        [DllImportAttribute(NDILib64Name, EntryPoint = "NDIlib_send_send_video", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void NDIlib64_send_send_video(IntPtr p_instance, ref NDIlib_video_frame_t p_video_data);
        [DllImportAttribute(NDILib32Name, EntryPoint = "NDIlib_send_send_video", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void NDIlib32_send_send_video(IntPtr p_instance, ref NDIlib_video_frame_t p_video_data);

        [DllImportAttribute(NDILib64Name, EntryPoint = "NDIlib_send_send_video_async", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void NDIlib64_send_send_video_async(IntPtr p_instance, ref NDIlib_video_frame_t p_video_data);
        [DllImportAttribute(NDILib32Name, EntryPoint = "NDIlib_send_send_video_async", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void NDIlib32_send_send_video_async(IntPtr p_instance, ref NDIlib_video_frame_t p_video_data);
        
        [DllImportAttribute(NDILib64Name, EntryPoint = "NDIlib_send_send_audio", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void NDIlib64_send_send_audio(IntPtr p_instance, ref NDIlib_audio_frame_t p_audio_data);
        [DllImportAttribute(NDILib32Name, EntryPoint = "NDIlib_send_send_audio", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void NDIlib32_send_send_audio(IntPtr p_instance, ref NDIlib_audio_frame_t p_audio_data);
                
        [DllImportAttribute(NDILib64Name, EntryPoint = "NDIlib_send_send_metadata", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void NDIlib64_send_send_metadata(IntPtr p_instance, ref NDIlib_metadata_frame_t p_meta_data);
        [DllImportAttribute(NDILib32Name, EntryPoint = "NDIlib_send_send_metadata", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void NDIlib32_send_send_metadata(IntPtr p_instance, ref NDIlib_metadata_frame_t p_meta_data);
                
        [DllImportAttribute(NDILib64Name, EntryPoint = "NDIlib_send_capture", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern NDIlib_frame_type_e NDIlib64_send_capture(IntPtr p_instance, ref NDIlib_metadata_frame_t p_meta_data, uint timeout_in_ms);
        [DllImportAttribute(NDILib32Name, EntryPoint = "NDIlib_send_capture", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern NDIlib_frame_type_e NDIlib32_send_capture(IntPtr p_instance, ref NDIlib_metadata_frame_t p_meta_data, uint timeout_in_ms);
                
        [DllImportAttribute(NDILib64Name, EntryPoint = "NDIlib_send_free_metadata", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void NDIlib64_send_free_metadata(IntPtr p_instance, ref NDIlib_metadata_frame_t p_meta_data);
        [DllImportAttribute(NDILib32Name, EntryPoint = "NDIlib_send_free_metadata", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void NDIlib32_send_free_metadata(IntPtr p_instance, ref NDIlib_metadata_frame_t p_meta_data);
                
        [DllImportAttribute(NDILib64Name, EntryPoint = "NDIlib_send_get_tally", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAsAttribute(UnmanagedType.U1)]
        private static extern bool NDIlib64_send_get_tally(IntPtr p_instance, ref NDIlib_tally_t p_tally, uint timeout_in_ms);
        [DllImportAttribute(NDILib32Name, EntryPoint = "NDIlib_send_get_tally", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAsAttribute(UnmanagedType.U1)]
        private static extern bool NDIlib32_send_get_tally(IntPtr p_instance, ref NDIlib_tally_t p_tally, uint timeout_in_ms);
        
        [DllImportAttribute(NDILib64Name, EntryPoint = "NDIlib_send_get_no_connections", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint NDIlib64_send_get_no_connections(IntPtr p_instance, uint timeout_in_ms);
        [DllImportAttribute(NDILib32Name, EntryPoint = "NDIlib_send_get_no_connections", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint NDIlib32_send_get_no_connections(IntPtr p_instance, uint timeout_in_ms);

        [DllImportAttribute(NDILib64Name, EntryPoint = "NDIlib_send_clear_connection_metadata", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void NDIlib64_send_clear_connection_metadata(IntPtr p_instance);
        [DllImportAttribute(NDILib32Name, EntryPoint = "NDIlib_send_clear_connection_metadata", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void NDIlib32_send_clear_connection_metadata(IntPtr p_instance);

        [DllImportAttribute(NDILib64Name, EntryPoint = "NDIlib_send_add_connection_metadata", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void NDIlib64_send_add_connection_metadata(IntPtr p_instance, ref NDIlib_metadata_frame_t p_meta_data);
        [DllImportAttribute(NDILib32Name, EntryPoint = "NDIlib_send_add_connection_metadata", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void NDIlib32_send_add_connection_metadata(IntPtr p_instance, ref NDIlib_metadata_frame_t p_meta_data);

#endregion
    }
}
