
using System;
using System.Runtime.Serialization;
using Yeti.MMedia;
using WaveLib;

namespace Yeti.MMedia.Mp3
{

  [Serializable]
	public class Mp3WriterConfig : Yeti.MMedia.AudioWriterConfig
	{
    private Lame.BE_CONFIG m_BeConfig;

    protected Mp3WriterConfig(SerializationInfo info, StreamingContext context)
      :base(info, context)
    {
      m_BeConfig = (Lame.BE_CONFIG)info.GetValue("BE_CONFIG", typeof(Lame.BE_CONFIG));
    }

    public Mp3WriterConfig(WaveFormat InFormat, Lame.BE_CONFIG beconfig)
      :base(InFormat)
    {
      m_BeConfig = beconfig;
    }

    public Mp3WriterConfig(WaveFormat InFormat)
      :this(InFormat, new Lame.BE_CONFIG(InFormat))
    {
    }

    public Mp3WriterConfig()
      :this(new WaveLib.WaveFormat(44100, 8, 2))
		{
		}
	
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("BE_CONFIG", m_BeConfig, m_BeConfig.GetType());
    }

    public Lame.BE_CONFIG Mp3Config
    {
      get
      {
        return m_BeConfig;
      }
      set
      {
        m_BeConfig = value;
      }
    }
  }
}
