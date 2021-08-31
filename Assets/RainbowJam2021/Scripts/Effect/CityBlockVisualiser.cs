using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBlockVisualiser : MonoBehaviour
{
    public enum PinnedPosType
    {
        Top,
        Bottom,
        Center
    }

    public FMODSpectrumData SpectrumDataRef;
    public float MinDB = -80;
    public float MaxDB = 0;

    public float MinScale = 5;
    public float MaxScale = 20;

    public int MinSpectralBandIndex = 0;
    public int MaxSpectralBandIndex = 10;

    public PinnedPosType PinnedPos = PinnedPosType.Bottom;

    public float LerpFactor = 0.1f;

    private float TargetScaleAdjust;
    private float CurrentScaleAdjust;
    private Vector3 StartingPos;

    void Start()
    {
        CurrentScaleAdjust = MinScale;
        TargetScaleAdjust = MinScale;
        StartingPos = transform.localPosition;
    }
    
    void Update()
    {
        if(SpectrumDataRef != null)
        {
            float spectralAverage = 0.0f;

            for(int spectralBand = MinSpectralBandIndex; spectralBand < MaxSpectralBandIndex; spectralBand++)
            {
                spectralAverage += SpectrumDataRef.SpectrumData[spectralBand];
            }

            spectralAverage /= (float)(MaxSpectralBandIndex - MinSpectralBandIndex);

            spectralAverage = spectralAverage.LinearToDecibels();

            TargetScaleAdjust = spectralAverage.RemapRangeClamped(MinDB, MaxDB, MinScale, MaxScale);

            CurrentScaleAdjust = Mathf.Lerp(CurrentScaleAdjust, TargetScaleAdjust, LerpFactor);

            var localScale = Vector3.one;
            var localPos = Vector3.zero;

            switch(PinnedPos)
            {
                case PinnedPosType.Top:
                    localScale.y = CurrentScaleAdjust;
                    localPos = -1.0f * transform.up * CurrentScaleAdjust/2;
                break;
                case PinnedPosType.Bottom:
                    localScale.y = CurrentScaleAdjust;
                    localPos = 1.0f * transform.up * CurrentScaleAdjust/2;
                break;
                case PinnedPosType.Center:
                    localScale.y = CurrentScaleAdjust;
                break;
            }

            transform.localScale = localScale;
            transform.localPosition = StartingPos + localPos;
        }
    }
}
