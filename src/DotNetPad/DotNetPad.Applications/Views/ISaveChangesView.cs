﻿using System.Waf.Applications;

namespace Waf.DotNetPad.Applications.Views;

public interface ISaveChangesView : IView
{
    void ShowDialog(object owner);

    void Close();
}
