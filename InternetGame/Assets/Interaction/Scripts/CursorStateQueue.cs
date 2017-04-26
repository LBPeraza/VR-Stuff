using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InternetGame
{
    public enum CursorState
    {
        Inactive = 30,
        Hovering = 20,
        Grabbing = 10,
        CutHover = 1,
        Unset = 0,
    }

    public class CursorStateQueue
    {
        protected Dictionary<CursorState, int> queue;
        protected CursorState currentValue;

        public CursorStateQueue()
        {
            queue = new Dictionary<CursorState, int> ();
            // Never run out of "inactive" states.
            queue[CursorState.Inactive] = int.MaxValue;
            currentValue = CursorState.Inactive;
        }

        /// <summary>
        /// Adds the given CursorState to the queue.
        /// </summary>
        /// <param name="c">The CursorState to enqueue</param>
        public void Enqueue(CursorState c)
        {
            if (!queue.ContainsKey(c))
            { 
                queue[c] = 0;
            }
            queue[c] += 1;

            if (c < currentValue)
            {
                // Update state if the enqueued value is higher priority
                // than the current state.
                currentValue = c;
            }
        }

        /// <summary>
        /// Deletes the given state from the queue.
        /// </summary>
        /// <param name="c">The state to be dequeued.</param>
        public void Dequeue(CursorState c)
        {
            if (queue.ContainsKey(c))
            {
                queue[c] -= 1;

                if (queue[c] <= 0)
                {
                    queue.Remove(c);

                    CursorState[] states = new CursorState[queue.Keys.Count];
                    queue.Keys.CopyTo(states, 0);

                    // Update current state to the new minimum.
                    currentValue = states.Min();
                }
            }
        }

        /// <summary>
        /// Gets the value from the queue with the lowest priority.
        /// </summary>
        /// <returns></returns>
        public CursorState Peek()
        {
            return currentValue;
        }
    }
}
