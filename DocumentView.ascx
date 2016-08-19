<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocumentView.ascx.cs" Inherits="TidyModules.DocumentExplorer.DocumentView" %>
<div id="documentExplorer">
    <div id="deFolders"></div>
    <div id="deFiles"></div>
</div>
<ul id="cmFolders"></ul>
<ul id="cmFiles"></ul>
<div id="deDialogMessage">
  <p id="message"></p>
</div>
<script type="text/javascript">
    $(document).ready(function() {
        tm_de.init($.ServicesFramework(<%= ModuleId %>));
    });
</script>