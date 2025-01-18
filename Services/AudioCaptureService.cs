using System;
using System.Runtime.InteropServices;
using ManagedBass;

namespace AvaloniaApplication1.Services;

public delegate void DataAvailableHandler(byte[] Buffer, int Length);
public class AudioCaptureService : IDisposable
{
    #region Private members
    
    private byte[] _buffer;
    
    private int _device, _handle;
    
    #endregion
    
    #region Public events
    
    public event DataAvailableHandler DataAvailable;
    
    #endregion
    
    #region Default Constructor
    public AudioCaptureService(RecordingDevice Device)
    {
        _device = Device.Index;
        
        Bass.RecordInit(_device);

        _handle = Bass.RecordStart(44100, 2, BassFlags.RecordPause, Procedure);
    }
    #endregion
    
    #region Capture Audio methods
    bool Procedure(int Handle, IntPtr Buffer, int Length, IntPtr User)
    {
        if (_buffer == null || _buffer.Length < Length)
            _buffer = new byte[Length];

        Marshal.Copy(Buffer, _buffer, 0, Length);

        DataAvailable?.Invoke(_buffer, Length);

        return true;
    }
    
    #endregion

    #region public control methods
    public void Start()
    {
        Bass.ChannelPlay(_handle);
    }

    public void Stop()
    {
        Bass.ChannelStop(_handle);
    }

    public void Dispose()
    {
        Bass.CurrentRecordingDevice = _device;

        Bass.RecordFree();
    }
    public void CaptureDefaultInput()
    {
        
    }
    
    #endregion
}