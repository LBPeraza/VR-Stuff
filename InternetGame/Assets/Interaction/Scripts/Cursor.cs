using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public enum CursorState
    {
        Inactive,
        Grabbing,
        Hovering,
        CutHover
    }

    public struct CursorEventArgs
    {
        public int senderId;
        public bool preventCursorModelChange;
    }

    public class Cursor : MonoBehaviour
    {
        public GameObject Controller;
        public Player Player;

        public bool HasLinkController;
        public bool IsRightHand;

        public CursorState State;

        public GameObject ArrowModel;
        public GameObject HandModel;
        public GameObject GrabModel;
        public GameObject ScissorsModel;

        public VRTK.VRTK_ControllerEvents Input;

        public static CursorEventArgs DefaultCursorEventArgs;

        private int? trackedInteractionId;

        private void TryFindController()
        {
            if (Controller == null)
            {
                Controller = IsRightHand ?
                    GameObject.FindGameObjectWithTag("RightController") :
                    GameObject.FindGameObjectWithTag("LeftController");
            }

            if (Controller != null)
            {
                Input = Controller.GetComponent<VRTK.VRTK_ControllerEvents>();
            }
        }

        public void Initialize(Player p, bool isRightHand)
        {
            DefaultCursorEventArgs.preventCursorModelChange = false;

            Player = p;
            IsRightHand = isRightHand;

            TryFindController();

            if (HasLinkController)
            {
                // Try to find LinkController script, and create one if necessary.
                var linkController = GetComponent<LinkController>();
                if (linkController == null)
                {
                    linkController = new LinkController();
                    linkController.transform.parent = this.transform.parent;
                }

                linkController.Initialize(Player, IsRightHand);
            }

            UpdateState(CursorState.Inactive);
        }

        public void OnEnterCut(CursorEventArgs args)
        {
            if (trackedInteractionId == null)
            {
                if (!args.preventCursorModelChange)
                {
                    UpdateState(CursorState.CutHover);
                }

                trackedInteractionId = args.senderId;
            }
        }

        public void OnExitCut(CursorEventArgs args)
        {
            if (trackedInteractionId == args.senderId)
            {
                if (!args.preventCursorModelChange)
                {
                    UpdateState(CursorState.Inactive);
                }

                trackedInteractionId = null;
            }
        }

        public void OnEnter(CursorEventArgs args)
        {
            if (trackedInteractionId == null)
            { 
                if (!args.preventCursorModelChange)
                {
                    UpdateState(CursorState.Hovering);
                }

                trackedInteractionId = args.senderId;
            }
        }

        public void OnGrab(CursorEventArgs args)
        {
            if (trackedInteractionId == args.senderId)
            {
                if (!args.preventCursorModelChange)
                {
                    UpdateState(CursorState.Grabbing);
                }
            }
        }

        public void OnDrop(CursorEventArgs args)
        {
            if (trackedInteractionId == args.senderId)
            {
                if (!args.preventCursorModelChange)
                {
                    UpdateState(CursorState.Hovering);
                }
            }
        }

        public void OnExit(CursorEventArgs args)
        {
            if (trackedInteractionId == args.senderId)
            {
                if (!args.preventCursorModelChange)
                {
                    UpdateState(CursorState.Inactive);
                }

                trackedInteractionId = null;
            }
        }

        private void UpdateState(CursorState newState)
        {
            switch (newState)
            {
                case CursorState.Inactive:
                    ArrowModel.SetActive(true);
                    HandModel.SetActive(false);
                    GrabModel.SetActive(false);
                    ScissorsModel.SetActive(false);
                    break;
                case CursorState.Hovering:
                    ArrowModel.SetActive(false);
                    HandModel.SetActive(true);
                    GrabModel.SetActive(false);
                    ScissorsModel.SetActive(false);
                    break;
                case CursorState.CutHover:
                    ArrowModel.SetActive(false);
                    HandModel.SetActive(false);
                    GrabModel.SetActive(false);
                    ScissorsModel.SetActive(true);
                    break;
                case CursorState.Grabbing:
                    ArrowModel.SetActive(false);
                    HandModel.SetActive(false);
                    GrabModel.SetActive(true);
                    ScissorsModel.SetActive(false);
                    break;
            }

            State = newState;
        }

        // Update is called once per frame
        void Update()
        {
            if (Controller != null)
            {
                this.transform.position = Controller.transform.position;
                this.transform.rotation = Controller.transform.rotation;
            } else
            {
                TryFindController();
            }
        }
    }
}

