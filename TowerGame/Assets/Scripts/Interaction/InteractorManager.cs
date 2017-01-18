using UnityEngine;
using System.Collections;

namespace TowerGame
{
    namespace Interaction
    {
        [RequireComponent(typeof(SteamVR_ControllerManager))]
        public class InteractorManager : MonoBehaviour
        {
            public Interactor InteractorPrefab;
            
            void Start()
            {
                var controllerManager = GetComponent<SteamVR_ControllerManager>();
                var leftHand = controllerManager.left;
                var rightHand = controllerManager.right;

                Interactor leftInteractor = (Interactor) Instantiate(InteractorPrefab, leftHand.transform);
                Interactor rightInteractor = (Interactor) Instantiate(InteractorPrefab, rightHand.transform);

                leftInteractor.transform.localPosition = Vector3.zero;
                rightInteractor.transform.localPosition = Vector3.zero;

                leftInteractor.Initialize(leftInteractor.gameObject, rightInteractor.gameObject,
                    true /* is left hand */, this.gameObject /* room transform */);
                rightInteractor.Initialize(leftInteractor.gameObject, rightInteractor.gameObject,
                    false /* is left hand */, this.gameObject /* room transform */);
            }
        }

    }
}

