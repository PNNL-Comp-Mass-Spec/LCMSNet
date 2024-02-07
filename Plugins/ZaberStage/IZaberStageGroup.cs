using ZaberStageControl;

namespace LcmsNetPlugins.ZaberStage
{
    /// <summary>
    /// Interface to support internal user interface interaction with generics
    /// </summary>
    public interface IZaberStageGroup
    {
        StageBase StageBase { get; }
        bool Emulation { get; }
        string PortName { get; set; }
    }
}
