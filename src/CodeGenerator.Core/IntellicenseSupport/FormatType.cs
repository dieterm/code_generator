namespace CodeGenerator.Core.IntellicenseSupport
{
    public enum FormatType
    {
        //
        // Summary:
        //     Specifies the default text drawing format.
        Text,
        //
        // Summary:
        //     Specifies the selected text format.
        SelectedText,
        //
        // Summary:
        //     Specifies the selected text drawing when window which holds the control loses
        //     focus.
        InactiveSelectedText,
        //
        // Summary:
        //     Specifies the display part of text in error color.
        Error,
        //
        // Summary:
        //     Specifies the special bookmark formatting.
        Bookmark,
        //
        // Summary:
        //     Specifies the display line of code as is on it set breakpoint.
        EnabledBreakPoint,
        //
        // Summary:
        //     Specifies the disabled breakpoint look and feel.
        DisabledBreakPoint,
        //
        // Summary:
        //     Specifies that breakpoint placed in wrong location.
        WrongBreakPoint,
        //
        // Summary:
        //     Specifies the current cursor position.
        CurrentStatement,
        //
        // Summary:
        //     Specifies the text of collapsed region caption.
        CollapsedText,
        //
        // Summary:
        //     Read only parts of text. Text marked in colors which say to user that code cannot
        //     be edited.
        ReadOnlyRegion,
        //
        // Summary:
        //     Specifies the special code which generated automatically by environment can be
        //     assigned to this format.
        WizardCode,
        //
        // Summary:
        //     Specifies the comment in parsed language.
        Comment,
        //
        // Summary:
        //     Specifies the operators and punctuators symbols.
        Operator,
        //
        // Summary:
        //     Specifies the keyword of language.
        KeyWord,
        //
        // Summary:
        //     Specifies the keyword which does not belong to language directly and used by
        //     pre-processing.
        PreprocessorKeyword,
        //
        // Summary:
        //     Specifies the strings.
        String,
        //
        // Summary:
        //     Specifies the one character symbols.
        SingleCharacter,
        //
        // Summary:
        //     Specifies the Unique resource identifier, mostly used for web URL and e-mails.
        URI,
        //
        // Summary:
        //     Specifies the number value in integer or float format.
        Number,
        //
        // Summary:
        //     Specifies the whitespace and tabs.
        Whitespace,
        //
        // Summary:
        //     Specifies all other formats which cannot be identified directly by control.
        Custom
    }
}
