using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public enum CursorState
    {
        Unset = 40,
        Inactive = 30,
        Hovering = 20,
        Grabbing = 10,
        CutHover = 0
    }

    public class CursorStateQueue
    {
        protected List<CursorState> queue;

        public CursorStateQueue()
        {
            queue = new List<CursorState>();
            queue.Add(CursorState.Inactive);
        }

        /// <summary>
        /// Adds the given CursorState to the queue.
        /// </summary>
        /// <param name="c">The CursorState to enqueue</param>
        public void Enqueue(CursorState c)
        {
            if (!queue.Contains(c))
            { 
                queue.Add(c);
                queue.Sort();
            }
        }

        /// <summary>
        /// Deletes the given state from the queue.
        /// </summary>
        /// <param name="c">The state to be dequeued.</param>
        public void Dequeue(CursorState c)
        {
            if (c != CursorState.Inactive && queue.Contains(c))
            {
                queue.Remove(c);
            }
        }

        /// <summary>
        /// Gets the value from the queue with the lowest priority.
        /// </summary>
        /// <returns></returns>
        public CursorState Peek()
        {
            return (CursorState) queue[0];
        }
    }
}
