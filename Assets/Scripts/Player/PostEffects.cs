using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PostEffects : MonoBehaviour {

    public PostProcessingProfile none;
    public PostProcessingProfile death;

    public Effect currentEffect;

    Effect lastFrameEffect;
    PostProcessingBehaviour postProcessBehaviour;

	// Use this for initialization
	public void Init () {
        postProcessBehaviour = GetComponent<PostProcessingBehaviour>();

        if (Save.current.combatData.currentHealth <= 0)
        {
            postProcessBehaviour.profile = death;
            currentEffect = lastFrameEffect = Effect.Dying;
        }
        else
        {
            postProcessBehaviour.profile = none;
            currentEffect = lastFrameEffect = Effect.None;
        }
    }

    private void Update()
    {
        //ako se promjenio post process
        if(currentEffect != lastFrameEffect)
        {
            //postavljanje novog post processa
            switch (currentEffect)
            {
                case Effect.None:
                    postProcessBehaviour.profile = none;
                    break;
                case Effect.Dying:
                    postProcessBehaviour.profile = death;
                    break;
                case Effect.Boss1:
                    break;
                case Effect.Boss2:
                    break;
                case Effect.Boss3:
                    break;
                case Effect.Boss4:
                    break;
                default:
                    break;
            }

            //reset proslog post processa
            ResetStates();
        }

        //Normalni update da post processi bolje izgledaju
        switch (currentEffect)
        {
            case Effect.None:
                break;
            case Effect.Dying:
                ColorGradingModel.Settings color = death.colorGrading.settings;
                if (death.colorGrading.settings.basic.saturation > 0)   color.basic.saturation -= Time.deltaTime;
                else                                                    color.basic.saturation = 0;
                death.colorGrading.settings = color;
                break;
            case Effect.Boss1:
                break;
            case Effect.Boss2:
                break;
            case Effect.Boss3:
                break;
            case Effect.Boss4:
                break;
            default:
                break;
        }
        lastFrameEffect = currentEffect;
    }

    private void ResetStates()
    {
        switch (lastFrameEffect)
        {
            case Effect.None:
                break;
            case Effect.Dying:
                death.colorGrading.Reset();
                break;
            case Effect.Boss1:
                break;
            case Effect.Boss2:
                break;
            case Effect.Boss3:
                break;
            case Effect.Boss4:
                break;
            default:
                break;
        }
    }

    public enum Effect
    {
        None,
        Dying,
        Boss1,
        Boss2,
        Boss3,
        Boss4
    }
    private void OnDisable()
    {
        ResetStates();
    }
}
