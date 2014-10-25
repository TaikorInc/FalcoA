using System;

namespace FalcoA.Core
{
    public static class Constant
    {
        #region Place Holders

        public const String UpdatablePlaceHolder = "@$";

        #endregion

        #region Prefixes

        public const String ParameterPrefix = "@";

        public const String DOMElementPrefix = "$";

        public const String RuntimePrefix = "#";

        public const String DOMIDPrefix = "ID";
        public const String DOMNamePrefix = "Name";
        public const String DOMXPathPrefix = "XPath";

        #endregion

        #region Phases



        #endregion

        #region Return Values

        public const String RVHttpRequestResult = "html";
        public const String RVCount = "count";

        #endregion

        #region Template

        public const String UseBrowserAttr = "UseBrowser";
        public const String FromAttr = "From";
        public const String ToAttr = "To";
        public const String StepAttr = "Step";
        public const String CountAttr = "Count";
        public const String SaveAttr = "Save";
        public const String UrlAttr = "Url";
        public const String HookAttr = "Hook";
        public const String ListIDAttr = "ListID";
        public const String ListBindingAttr = "ListBinding";
        public const String NameAttr = "Name";
        public const String FileUrlTagAttr = "FileTag";
        public const String UrlTagAttr = "UrlTag";

        public const String DocumentTypeAttr = "Type";
        public const String FileExtensionAttr = "Extension";

        public const String TemplateNode = "Template";
        public const String NestedNode = "Nested";
        public const String NestedRegexNode = "NestedRegex";
        public const String RegexNode = "Regex";
        public const String BaseXPathNode = "BaseXPath";
        public const String XPathNode = "XPath";
        public const String WaitMillisecondsNode = "WaitMilliseconds";
        public const String UrlRegexNode = "UrlRegex";
        public const String ContentRegexNode = "ContentRegex";
        public const String UrlNode = "Url";
        public const String DataNode = "Data";
        public const String ScanNode = "Scan";
        public const String RequestNode = "Request";
        public const String ParseNode = "Parse";
        public const String TargetNode = "Target";
        public const String ClickNode = "Click";
        public const String ListNode = "List";
        public const String MakeListNode = "MakeList";
        public const String FirstPageNode = "FirstPage";
        public const String PatternNode = "Pattern";
        public const String UsernameNode = "Username";
        public const String PasswordNode = "Password";
        public const String LoginNode = "Login";
        public const String LogoutNode = "Logout";
        public const String DirectoryNode = "Directory";
        public const String DownloadNode = "Download";

        public const String ParameterProviderNode = "ParameterProvider";

        #endregion

        #region Misc

        public const String PhaseCreatorMethod = "Create";

        #endregion

        #region Utility

        public static Boolean True(String val)
        {
            return val == "1" || val == "Yes" || val == "yes";
        }

        #endregion
    }
}
