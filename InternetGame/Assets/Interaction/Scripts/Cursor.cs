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

        public bool IsRightHand;
        public bool ControllerInitialized;

        public CursorState State;
        public Stack<CursorState> States;

        public GameObject ArrowModel;
        public GameObject HandModel;
        public GameObject GrabModel;
        public GameObject ScissorsModel;

        public VRTK.VRTK_ControllerEvents Input;
        public VRTK.VRTK_ControllerActions ControllerActions;
        public delegate void OnControllerFoundHandler(VRTK.VRTK_ControllerEvents input);
        public event OnControllerFoundHandler OnControllerFound;

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
                ControllerActions = Controller.GetComponent<VRTK.VRTK_ControllerActions>();

                if (Input != null && ControllerActions != null)
                {
                    ControllerInitialized = true;

                    if (OnControllerFound != null)
                    {
                        OnControllerFound.Invoke(Input);
                    }
                }
            }
        }

        public void Initialize(Player p, bool isRightHand)
        {
            DefaultCursorEventArgs.preventCursorModelChange = false;

            States = new Stack<CursorState>();

            Player = p;
            IsRightHand = isRightHand;

            TryFindController();

            UpdateState(CursorState.Inactive);
        }

        public void OnEnterCut(CursorEventArgs args)
        {
            if (trackedInteractionId == null)
            {
                if (!args.preventCursorModelChange)
                {
                    States.Push(State);
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
                    UpdateState(States.Pop());
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
                    States.Push(State);
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
                    States.Push(State);
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
                    UpdateState(States.Pop());
                }
            }
        }

        public void OnExit(CursorEventArgs args)
        {
            if (trackedInteractionId == args.senderId)
            {
                if (!args.preventCursorModelChange)
                {
                    UpdateState(States.Pop());
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
            if (!ControllerInitialized)
            {
                TryFindController();
            }

            if (Controller != null)
            {
                this.transform.position = Controller.transform.position;
                this.transform.rotation = Controller.transform.rotation;
            }
        }
    }
}

