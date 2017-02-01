using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public enum CursorState
    {
        Inactive,
        Grabbing,
        Hovering
    }

    public struct CursorEventArgs
    {
        public int senderId;
        public bool preventCursorModelChange;
    }

    public class Cursor : MonoBehaviour
    {
        public Transform follow;
        public Player Player;

        public bool HasLinkController;
        public bool IsRightHand;

        public CursorState State;

        public GameObject ArrowModel;
        public GameObject HandModel;
        public GameObject GrabModel;

        public static CursorEventArgs DefaultCursorEventArgs;

        private int? trackedInteractionId;

        public void Initialize(Player p, bool isRightHand)
        {
            DefaultCursorEventArgs.preventCursorModelChange = false;

            Player = p;
            IsRightHand = isRightHand;

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
                    break;
                case CursorState.Hovering:
                    ArrowModel.SetActive(false);
                    HandModel.SetActive(true);
                    GrabModel.SetActive(false);
                    break;
                case CursorState.Grabbing:
                    ArrowModel.SetActive(false);
                    HandModel.SetActive(false);
                    GrabModel.SetActive(true);
                    break;
            }

            State = newState;
        }

        // Update is called once per frame
        void Update()
        {
            if (follow != null)
            {
                this.transform.position = follow.transform.position;
                this.transform.rotation = follow.transform.rotation;
            }
        }
    }
}

