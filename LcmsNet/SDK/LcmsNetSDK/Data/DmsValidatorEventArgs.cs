using System;
using System.Windows.Forms;

public class DMSValidatorEventArgs : EventArgs
{
    public DMSValidatorEventArgs(string name, Keys modifier)
    {
        Name = name;
        Modifiers = modifier;
    }

    public string Name { get; private set; }

    public Keys Modifiers { get; private set; }
}