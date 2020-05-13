namespace PixelEngine
{
	public enum Key
	{
		A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
		K0, K1, K2, K3, K4, K5, K6, K7, K8, K9,
		F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12,
		Up, Down, Left, Right,
		Space, Tab, Shift, Control, Insert, Delete, Home, End, PageUp, PageDown,
		Back, Escape, Enter, Pause, Scroll, Any, None
	}

	public enum Mouse
	{
		Left,
		Middle,
		Right,
		Any,
		None
	}

	public enum Scroll
	{
		Up = -1,
		None = 0,
		Down = 1
	}

	public enum SortMode
	{
		Deferred,
		BackToFront,
		FrontToBack
	}

    public enum PixelMode
    {
        Normal,
        Addetive,
        Negation,
        Multiply,
        Xor,
        Mask,
        Alpha,
        Custom
    }

    #region Flags
    [System.Flags]
    public enum SpriteEffects
    {
        None = 0b000,

        FlipHorizontally = 0b001,
        FlipVertically = 0b010,
        FlipBoth = 0b11
    }

    [System.Flags]
    public enum Direction
    {
        Undefined = 0b000001,

        North = 0b00010,
        South = 0b00100,
        West  = 0b01000,
        East  = 0b10000,

        Up    = North,
        Down  = South,
        Left  = West,
        Right = East,

        Vertically   = North | South,
        Horizontally = West  | East
    }

    [System.Flags]
    public enum ConditionType
    {
        /// <summary>
        /// Logical AND, can not be combined with other ConditionTypes
        /// </summary>
        AND = 0b0000,
        /// <summary>
        /// Less than
        /// </summary>
        LSS = 0b0001,
        /// <summary>
        /// Greater than
        /// </summary>
        QTR = 0b0010,
        /// <summary>
        /// Is Not
        /// </summary>
        NOT = 0b0100,
        /// <summary>
        /// Equals
        /// </summary>
        EQU = 0b1000,
        /// <summary>
        /// Less or Equals than
        /// </summary>
        LEQ = 0b1001,
        /// <summary>
        /// Greater or Equals than
        /// </summary>
        GEQ = 0b1010,
        /// <summary>
        /// Not Equals
        /// </summary>
        NEQ = 0b1100
    }
    #endregion
}
