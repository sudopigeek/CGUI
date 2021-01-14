using System.Collections;
using System.Collections.Generic;

namespace CGUI
{
    /// <summary>
    /// Represents a list of controls.
    /// </summary>
    public class ControlList : IEnumerable
    {
        internal static List<Control> Controls;
        private int GetControlIndex(Control control)
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i].controlType == control.controlType && Controls[i].X == control.X && Controls[i].Y == control.Y)
                {
                    return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// Starts a new instance of the ControlList class.
        /// </summary>
        public ControlList()
        {
            Controls = new List<Control>();
        }
        /// <summary>
        /// Gets the enumerator for the ControlList.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return new ControlEnumerator(Controls);
        }
        /// <summary>
        /// Gets the control at the specified index.
        /// </summary>
        /// <param name="index">The index of the desired control.</param>
        /// <returns></returns>
        public Control this[int index]
        {
            get
            {
                return Controls[index];
            }
        }
        /// <summary>
        /// Gets the number of controls in the ControlList.
        /// </summary>
        public int Count
        {
            get
            {
                return Controls.Count;
            }
        }
        /// <summary>
        /// Adds a new control to the end of the ControlList.
        /// </summary>
        /// <param name="control">The control to add.</param>
        public void Add(Control control) 
        {
            VGADriver.DrawControl(control, true);
            Controls.Add(control); 
        }
        /// <summary>
        /// Removes a control from the ControlList. If the currently focused control is removed, the screen's first control is automatically focused.
        /// </summary>
        /// <param name="control">The control to remove.</param>
        public void Remove(Control control) 
        {
            if (GetControlIndex(control) > -1)
            {
                VGADriver.DeleteControl(control);
                Controls.RemoveAt(GetControlIndex(control));
            }        
        }
        /// <summary>
        /// Removes a control from the ControlList and sets focus to a different control.
        /// </summary>
        /// <param name="control">The control to remove.</param>
        /// <param name="focusNext">The control to set focus to after removing the initial control.</param>
        public void Remove(Control control, Control focusNext)
        {
            if (GetControlIndex(control) > -1)
            {
                VGADriver.DeleteControl(control, focusNext);
                Controls.RemoveAt(GetControlIndex(control));
            }
        }
        /// <summary>
        /// Removes a control from the ControlList at the specified index.
        /// </summary>
        /// <param name="index">The index to remove at.</param>
        public void RemoveAt(int index) 
        {
            if (index > -1 && index < Controls.Count)
            {
                VGADriver.DeleteControl(Controls[index]);
                Controls.RemoveAt(index);
            }        
        }
        /// <summary>
        /// Removes a range of controls from the ControlList.
        /// </summary>
        /// <param name="index">The index at which to start at.</param>
        /// <param name="count">The number of controls to remove.</param>
        public void RemoveRange(int index, int count) 
        {
            if (index > -1 && index < Controls.Count && (index + count) < Controls.Count)
            {
                for (int i = index; i < (index + count); i++)
                {
                    VGADriver.DeleteControl(Controls[i]);
                }
                Controls.RemoveRange(index, count);
            }                    
        }
        /// <summary>
        /// Removes all controls from the ControlList.
        /// </summary>
        public void Clear() { Controls.Clear(); }
        /// <summary>
        /// Determines whether an item is in the ControlList.
        /// </summary>
        /// <param name="control">The control to check for.</param>
        /// <returns></returns>
        public bool Contains(Control control) 
        {
            if (GetControlIndex(control) > -1)
                return true;
            else
                return false;
        }
        /// <summary>
        /// Searches for the specified control and returns the zero-based index of the first occurrence in the ControlList.
        /// </summary>
        /// <param name="control">The control to search for.</param>
        /// <returns></returns>
        public int IndexOf(Control control) 
        {
            return GetControlIndex(control);
        }
        /// <summary>
        /// Inserts a control into the ControlList at the specified index.
        /// </summary>
        /// <param name="index">The index to insert the control at.</param>
        /// <param name="control">The control to insert.</param>
        public void Insert(int index, Control control) 
        { 
            Controls.Insert(index, control); 
        }
        /// <summary>
        /// Copies the elements of the ControlList to a new array.
        /// </summary>
        /// <returns></returns>
        public Control[] ToArray() { return Controls.ToArray(); }
    }
    internal class ControlEnumerator : IEnumerator
    {
        private List<Control> Controls;
        private int Counter = -1;
        public ControlEnumerator(List<Control> controls)
        {
            Controls = controls;
        }
        public bool MoveNext()
        {
            Counter++;
            return Counter != Controls.Count;
        }
        public void Reset()
        {
            
        }
        public object Current
        {
            get
            {
                return Controls[Counter];
            }
        }
    }
}
