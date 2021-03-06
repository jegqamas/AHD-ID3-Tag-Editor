/* This file is part of AHD ID3 Tag Editor (AITE)
 * A program that edit and create ID3 Tag.
 *
 * Copyright © Alaa Ibrahim Hadid 2012 - 2022
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * Author email: mailto:alaahadidfreeware@gmail.com
 * 
 */
using System;

namespace MLV
{
    /// <summary>
    /// Arguments for item double click events
    /// </summary>
    public class ManagedListViewItemDoubleClickArgs : EventArgs
    {
        /// <summary>
        /// Arguments for item double click events
        /// </summary>
        /// <param name="itemIndex">The clicked item index</param>
        public ManagedListViewItemDoubleClickArgs(int itemIndex)
        {
            this.index = itemIndex;
        }

        private int index = 0;
        /// <summary>
        /// Get the clicked item index
        /// </summary>
        public int ClickedItemIndex
        {
            get { return index; }
        }
    }
}
