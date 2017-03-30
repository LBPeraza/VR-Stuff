using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
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
        public bool IsPrimary { get; set; }

        public CursorStateQueue CursorStates;

        public GameObject ArrowModel;
        public GameObject HandModel;
        public GameObject GrabModel;
        public GameObject ScissorsModel;

        public VRTK.VRTK_ControllerEvents Input;
        public VRTK.VRTK_ControllerActions ControllerActions;
        public delegate void OnControllerFoundHandler(VRTK.VRTK_ControllerEvents input);
        public event OnControllerFoundHandler ControllerFound;

        public static CursorEventArgs DefaultCursorEventArgs;

        private void TryFindController()
        {
            if (Controller == null)
            {
                Controller = IsRightHand ?
                    GameObject.FindGameObjectWithTag("RightController") :
                    GameObject.FindGameObjectWithTag("LeftController");
            }

            if (Controller == null)
            {
                Controller = IsRightHand ?
                    GameObject.Find("[CameraRig]/Controller (right)") :
                    GameObject.Find("[CameraRig]/Controller (left)");
            }

            if (Controller != null)
            {
                Input = Controller.GetComponent<VRTK.VRTK_ControllerEvents>();
                ControllerActions = Controller.GetComponent<VRTK.VRTK_ControllerActions>();

                if (Input != null && ControllerActions != null)
                {
                    ControllerInitialized = true;

                    if (ControllerFound != null)
                    {
                        ControllerFound.Invoke(Input);
                    }
                }
            }
        }

        public void Initialize(Player p, bool isRightHand, bool isPrimary)
        {
            ControllerInitialized = false;
            DefaultCursorEventArgs.preventCursorModelChange = false;

            CursorStates = new CursorStateQueue();

            Player = p;
            IsRightHand = isRightHand;
            IsPrimary = isPrimary;

            ControllerFound += OnInputReady;

            TryFindController();

            UpdateState(CursorState.Inactive);
        }

        private void OnInputReady(VRTK.VRTK_ControllerEvents input)
        {
            input.AliasGrabOff += ControllerSqueezed;
        }

        private void ControllerSqueezed(object sender, VRTK.ControllerInteractionEventArgs e)
        {
            Player.SetPrimaryCursor(this);
        }

        public void OnEnterCut(CursorEventArgs args)
        {
            if (!args.preventCursorModelChange)
            {
                //CursorStates.Enqueue(CursorState.CutHover);
                //UpdateState(CursorStates.Peek());
            }
        }

        public void OnExitCut(CursorEventArgs args)
        {

            if (!args.preventCursorModelChange)
            {
                //CursorStates.Dequeue(CursorState.CutHover);
                //UpdateState(CursorStates.Peek());
            }
        }

        public void OnEnter(CursorEventArgs args)
        {
            if (!args.preventCursorModelChange)
            {
                CursorStates.Enqueue(CursorState.Hovering);
                UpdateState(CursorStates.Peek());
            }
        }

        public void OnGrab(CursorEventArgs args)
        {
            if (!args.preventCursorModelChange)
            {
                CursorStates.Enqueue(CursorState.Grabbing);
                UpdateState(CursorStates.Peek());
            }
        }

        public void OnDrop(CursorEventArgs args)
        {
            if (!args.preventCursorModelChange)
            {
                CursorStates.Dequeue(CursorState.Grabbing);
                UpdateState(CursorStates.Peek());
            }
        }

        public void OnExit(CursorEventArgs args)
        {
            if (!args.preventCursorModelChange)
            {
                CursorStates.Dequeue(CursorState.Hovering);
                UpdateState(CursorStates.Peek());
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

