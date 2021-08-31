using System;
using UnityEngine;
using System.Runtime.InteropServices;

class FMODSpectrumData : MonoBehaviour
{
    private FMOD.DSP fft;

    public int WindowSize = 128;
    public string BusPath = "bus:/Music";
    public float[] SpectrumData;

    void Start()
    {
        SpectrumData = new float[WindowSize];

        if (FMODUnity.RuntimeManager.CoreSystem.createDSPByType(FMOD.DSP_TYPE.FFT, out fft) == FMOD.RESULT.OK)
        {
            fft.setParameterInt((int)FMOD.DSP_FFT.WINDOWTYPE, (int)FMOD.DSP_FFT_WINDOW.HANNING);
            fft.setParameterInt((int)FMOD.DSP_FFT.WINDOWSIZE, WindowSize * 2);

            FMOD.Studio.Bus selectedBus = FMODUnity.RuntimeManager.GetBus(BusPath);
            if (selectedBus.hasHandle())
            {
                selectedBus.lockChannelGroup();
                FMODUnity.RuntimeManager.StudioSystem.flushCommands();

                if (selectedBus.getChannelGroup(out var channelGroup) == FMOD.RESULT.OK)
                {
                    if (channelGroup.addDSP(FMOD.CHANNELCONTROL_DSP_INDEX.HEAD, fft) != FMOD.RESULT.OK)
                    {
                        Debug.LogWarningFormat("FMOD: Unable to add FFT to the Channel Group");
                    }
                }
                else
                {
                    Debug.LogWarningFormat("FMOD: Unable to get Channel Group from the selected bus");
                }
                selectedBus.unlockChannelGroup();
            }
            else
            {
                Debug.LogWarningFormat("FMOD: Unable to get the selected bus");
            }
        }
        else
        {
            Debug.LogWarningFormat("FMOD: Unable to create FMOD.DSP_TYPE.FFT");
        }
    }

    void Update()
    {
        if (fft.hasHandle())
        {
            if (fft.getParameterData((int)FMOD.DSP_FFT.SPECTRUMDATA, out var unmanagedData, out var length) == FMOD.RESULT.OK)
            {
                var fftData = Marshal.PtrToStructure<FMOD.DSP_PARAMETER_FFT>(unmanagedData);
                if (fftData.numchannels > 0)
                {
                    fftData.getSpectrum(0, ref SpectrumData);
                }
            }
        }
    }

    void OnDestroy()
    {
        FMOD.Studio.Bus selectedBus = FMODUnity.RuntimeManager.GetBus(BusPath);
        if (selectedBus.hasHandle())
        {
            if (selectedBus.getChannelGroup(out var channelGroup) == FMOD.RESULT.OK)
            {
                if(fft.hasHandle())
                {
                    channelGroup.removeDSP(fft);
                }
            }
        }
    }
}
