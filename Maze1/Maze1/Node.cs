using System.Drawing;

namespace Maze1
{
    /// <summary>
    /// Class representing a node
    /// </summary>
    internal class Node
    {
        // Class variables
        internal Point Position { get; set; }
        internal Node Parent { get; set; }
        
        /// <summary>
        /// Default constructor
        /// </summary>
        internal Node()
        {

        }

        /// <summary>
        /// Constructor for specific point in array
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        internal Node(int x, int y)
        {
            Position = new Point(x, y);
        }
    }
}