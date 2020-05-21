

using System;
using System.IO;
using Yeti.Lame;
using Yeti.MMedia;
using WaveLib;

namespace Yeti.MMedia.Mp3
{

  public class Mp3Writer :  AudioWriter
  {
    private bool closed = false;
    private BE_CONFIG m_Mp3Config = null;
    private uint m_hLameStream = 0;
    private uint m_InputSamples = 0;
    private uint m_OutBufferSize = 0;
    private byte[] m_InBuffer = null;
    private int m_InBufferPos = 0;
    private byte[] m_OutBuffer = null;

    public Mp3Writer(Stream Output, WaveFormat InputDataFormat)
      :this(Output, InputDataFormat, new BE_CONFIG(InputDataFormat))
    {
    }


    public Mp3Writer(Stream Output, Mp3WriterConfig cfg)
      :this(Output, cfg.Format, cfg.Mp3Config)
    {
    }


    public Mp3Writer(Stream Output, WaveFormat InputDataFormat, BE_CONFIG Mp3Config)
      :base(Output, InputDataFormat)
    {
      try
      {
        m_Mp3Config = Mp3Config;
        uint LameResult = Lame_encDll.beInitStream(m_Mp3Config, ref m_InputSamples, ref m_OutBufferSize, ref m_hLameStream);
        if ( LameResult != Lame_encDll.BE_ERR_SUCCESSFUL)
        {
          throw new ApplicationException(string.Format("Lame_encDll.beInitStream failed with the error code {0}", LameResult));
        }
        m_InBuffer = new byte[m_InputSamples*2]; 
        m_OutBuffer = new byte[m_OutBufferSize];
      }
      catch
      {
        base.Close();
        throw;
      }
    }

    public BE_CONFIG Mp3Config
    {
      get
      {
        return m_Mp3Config;
      }
    }

    protected override int GetOptimalBufferSize()
    {
      return m_InBuffer.Length;
    }

    public override void Close()
    {
      if (!closed)
      {
        try
        {
          uint EncodedSize = 0;
          if ( m_InBufferPos > 0)
          {
            if ( Lame_encDll.EncodeChunk(m_hLameStream, m_InBuffer, 0, (uint)m_InBufferPos, m_OutBuffer, ref EncodedSize) == Lame_encDll.BE_ERR_SUCCESSFUL )
            {
              if ( EncodedSize > 0)
              {
                base.Write(m_OutBuffer, 0, (int)EncodedSize);
              }
            }
          }
          EncodedSize = 0;
          if (Lame_encDll.beDeinitStream(m_hLameStream, m_OutBuffer, ref EncodedSize) == Lame_encDll.BE_ERR_SUCCESSFUL )
          {
            if ( EncodedSize > 0)
            {
              base.Write(m_OutBuffer, 0, (int)EncodedSize);
            }
          }
        }
        finally
        {
          Lame_encDll.beCloseStream(m_hLameStream);
        }
      }
      closed = true;
      base.Close ();
    }
  
  
    
    public override void Write(byte[] buffer, int index, int count)
    {
      int ToCopy = 0;
      uint EncodedSize = 0;
      uint LameResult;
      while (count > 0)
      {
        if ( m_InBufferPos > 0 ) 
        {
          ToCopy = Math.Min(count, m_InBuffer.Length - m_InBufferPos);
          Buffer.BlockCopy(buffer, index, m_InBuffer, m_InBufferPos, ToCopy);
          m_InBufferPos += ToCopy;
          index += ToCopy;
          count -= ToCopy;
          if (m_InBufferPos >= m_InBuffer.Length)
          {
            m_InBufferPos = 0;
            if ( (LameResult=Lame_encDll.EncodeChunk(m_hLameStream, m_InBuffer, m_OutBuffer, ref EncodedSize)) == Lame_encDll.BE_ERR_SUCCESSFUL )
            {
              if ( EncodedSize > 0)
              {
                base.Write(m_OutBuffer, 0, (int)EncodedSize);
              }
            }
            else
            {
              throw new ApplicationException(string.Format("Lame_encDll.EncodeChunk failed with the error code {0}", LameResult));
            }
          }
        }
        else
        {
          if (count >= m_InBuffer.Length)
          {
            if ( (LameResult=Lame_encDll.EncodeChunk(m_hLameStream, buffer, index, (uint)m_InBuffer.Length, m_OutBuffer, ref EncodedSize)) == Lame_encDll.BE_ERR_SUCCESSFUL )
            {
              if ( EncodedSize > 0)
              {
                base.Write(m_OutBuffer, 0, (int)EncodedSize);
              }
            }
            else
            {
              throw new ApplicationException(string.Format("Lame_encDll.EncodeChunk failed with the error code {0}", LameResult)); 
            }
            count -= m_InBuffer.Length;
            index += m_InBuffer.Length;
          }
          else
          {
            Buffer.BlockCopy(buffer, index, m_InBuffer, 0, count);
            m_InBufferPos = count;
            index += count;
            count = 0;
          }
        }
      }
    }
  
    
    public override void Write(byte[] buffer)
    {
      this.Write (buffer, 0, buffer.Length);
    }
  
    protected override AudioWriterConfig GetWriterConfig()
    {
      return new Mp3WriterConfig(m_InputDataFormat, Mp3Config);
    }
  }
}