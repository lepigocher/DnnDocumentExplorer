; (function (de, $, undefined) {
    // Local variables
    var sf = {},
        services = null,
        locale = null,
        sizes = null,
        showIcon = false,
        fileManagement = false,
        hasFilter = false,
        resetFilters = false,
        dlgWindow = null,
        dlgMessage = null,
        deFolders = null,
        cmFolders = null,
        deFiles = null,
        cmFiles = null,
        selectedNode = null,
        command = null,
        itemTmpl = "<li><a id='{0}' data-icon='{1}'>{2}</a></li>";

    // Declare ItemCommand object
    function ItemCommand(path, files, flag, toPath) {
        this.path = path;
        this.files = files != null ? (files instanceof Array ? files : [files]) : [];
        this.flag = typeof (flag) === "boolean" ? flag : false;
        this.toPath = toPath;
    }

    // Get file names from selected files
    function getFileNames(files) {
        var names = [];

        if (files instanceof Array) {
            for (var i = 0; i < files.length; i++) {
                names.push(files[i].name);
            }
        } else {
            names.push(files.name);
        }

        return names;
    }

    // Create spinner
    function createSpinner(source) {
        var spinner = $("<div class='spinner'><i class='fa fa-spinner fa-pulse fa-3x fa-fw'></i><span class='sr-only'>" + locale["loading"] + "</span></div>");
        var sourceHeigth = source.outerHeight();

        if (sourceHeigth == 0) {
            source.children().each(function () {
                var elementHeight = $(this).outerHeight();

                if (elementHeight > sourceHeigth)
                    sourceHeigth = elementHeight;
            });
        }

        spinner.find("i").css("margin-top", ((sourceHeigth / 2) - 20) + "px");
        spinner.height(sourceHeigth);
        spinner.width(source.width());
        spinner.prependTo(source);

        return spinner;
    }

    // Retreive then display folders
    function getFolders(ui, response) {
        var path = "",
            spinner = createSpinner(deFolders);

        if (ui.data && ui.data.path != null)
            path = ui.data.path;

        var url = services.format("Folders"),
            cmd = new ItemCommand(path);

        $.ajax({
            type: "GET",
            url: url,
            dataType: "json",
            data: cmd,
            context: this,
            beforeSend: sf.setModuleHeaders
        }).done(function (result, status, xhr) {
            // Fill tree node with data
            if (result != null)
                response.call(this, result, ui.node);

            // If it's the 'Root' select this node
            if (path == "") {
                var container = this.rootContainer;
                var first = container.find(".ui-treenode-parent:first");
                this.selectNode(first);
            }
        }).fail(function (xhr, result, status) {
            showError(status, xhr.responseText);
        }).complete(function () {
            spinner.remove();
        });
    }

    // Retreive then display files for the specified folder
    function getFiles(path) {
        var spinner = createSpinner(deFiles),
            url = services.format("Files"),
            cmd = new ItemCommand(path, null, showIcon);

        $.ajax({
            type: "GET",
            url: url,
            dataType: "json",
            data: cmd,
            context: this,
            beforeSend: sf.setModuleHeaders
        }).done(function (result, status, xhr) {
            // Refresh data table with new files
            deFiles.puidatatable("option", "datasource", result);
            if (hasFilter) {
                // Clear filters if required
                if (resetFilters) {
                    deFiles.find(".ui-column-filter").each(function () {
                        $(this).val("");
                    });
                }
                // Apply filters if any
                deFiles.puidatatable("filter");
            }
        }).fail(function (xhr, result, status) {
            showError(status, xhr.responseText);
        }).complete(function () {
            spinner.remove();
        });
    }

    // Emit a get command to the specified service
    function getCommand(service, cmd) {
        return $.ajax({
            type: "GET",
            url: services.format(service),
            dataType: "json",
            data: cmd,
            beforeSend: sf.setModuleHeaders
        });
    }

    // Emit a post command to the specified service
    function postCommand(service, cmd) {
        return $.ajax({
            type: "POST",
            url: services.format(service),
            dataType: "json",
            data: cmd,
            beforeSend: sf.setModuleHeaders
        });
    }

    // Compose file name with icon
    function composeName(row, options) {
        var name = "{0} {1}";

        return name.format(row.icon, row.name);
    }

    // Convert file size to text
    function bytesToSize(row, options) {
        var bytes = row.size;

        if (bytes == 0)
            return lr["ZeroByte"];

        var i = parseInt(Math.floor(Math.log(bytes) / Math.log(1024)));

        return Math.round(bytes / Math.pow(1024, i), 2) + " " + sizes[i];
    }

    // Format date
    function formatDate(row) {
        var date = new Date(row.modified);

        return date.toLocaleDateString() + " " + date.toLocaleTimeString();
    }

    // Show dialog message
    function showMessage(message) {
        dlgMessage.text(message);
        dlgWindow.dialog("open");
    }

    // Show error message
    function showError(status, error) {
        var message = "{0}: {1}";

        showMessage(message.format(status, error));
    }

    // Show confirmation message
    function showConfirm(message, obj) {
        var defer = $.Deferred();

        $("<div></div>")
            .html(message)
                .dialog({
                    modal: true,
                    minWidth: 350,
                    title: locale["confirmTitle"],
                    dialogClass: "dnnFormPopup",
                    buttons: [
                    {
                        text: locale["dialogOk"],
                        click: function () {
                            defer.resolve(obj);
                            $(this).dialog("close");
                        }
                    },
                    {
                        text: locale["dialogCancel"],
                        click: function () {
                            defer.reject();
                            $(this).dialog("close");
                        }
                    }
                    ],
                    close: function () {
                        $(this).dialog("destroy").remove()
                    }
                });

        return defer.promise();
    }

    // Get file name dialog
    function getName(title, value) {
        var defer = $.Deferred();
        var html = "<div><label for='txtItemName'>" + locale["itemName"] + "&nbsp;</label><input type='text' name='txtItemName' id='txtItemName' class='ui-inputtext ui-widget-content ui-corner-all' value='" + value + "'></div>";

        $(html).dialog({
            modal: true,
            minWidth: 350,
            title: title,
            dialogClass: "dnnFormPopup",
            buttons: [
                    {
                        text: locale["dialogOk"],
                        click: function () {
                            var newName = $(this).find("#txtItemName").val().trim();

                            if (newName != "")
                                defer.resolve(newName);
                            else
                                defer.reject();

                            $(this).dialog("close");
                        }
                    },
                    {
                        text: locale["dialogCancel"],
                        click: function () {
                            defer.reject();

                            $(this).dialog("close");
                        }
                    }
            ],
            close: function () {
                $(this).dialog("destroy").remove();
            }
        });

        return defer.promise();
    }

    // Build files contextual menu
    function buildFileMenu() {
        var options = [];

        if (fileManagement) {
            options.push(itemTmpl.format("cmfSelectAll", "fa-hand-lizard-o", locale["itemSelectAll"]));
            options.push(itemTmpl.format("cmfCut", "fa-scissors", locale["itemCut"]));
            options.push(itemTmpl.format("cmfCopy", "fa-copy", locale["itemCopy"]));
            options.push(itemTmpl.format("cmfPaste", "fa-paste", locale["itemPaste"]));
            options.push(itemTmpl.format("cmfRename", "fa-edit", locale["itemRename"]));
            options.push(itemTmpl.format("cmfDelete", "fa-trash", locale["itemDelete"]));
        }

        options.push(itemTmpl.format("cmfOpen", "fa-external-link", locale["fileOpen"]));
        options.push(itemTmpl.format("cmfDownload", "fa-download", locale["fileDownload"]));
        options.push(itemTmpl.format("cmfClipboard", "fa-link", locale["fileClipboard"]));

        if (fileManagement) {
            options.push(itemTmpl.format("cmfPack", "fa-compress", locale["filePack"]));
            options.push(itemTmpl.format("cmfUnpack", "fa-expand", locale["fileUnpack"]));
        }

        // Append menu options
        cmFiles.append(options);
        // Event delegation
        cmFiles.on("click", "li a", fileMenuHandler);
        // Create contextual menu
        cmFiles.puicontextmenu({
            target: deFiles
        });
    }

    // Build folders contextual menu
    function builFolderMenu(synchronizeFolder) {
        var options = [];

        if (fileManagement) {
            options.push(itemTmpl.format("cmfdCreate", "fa-plus", locale["folderCreate"]));
            options.push(itemTmpl.format("cmfdCut", "fa-scissors", locale["itemCut"]));
            options.push(itemTmpl.format("cmfdPaste", "fa-paste", locale["itemPaste"]));
            options.push(itemTmpl.format("cmfdRename", "fa-edit", locale["itemRename"]));
            options.push(itemTmpl.format("cmfdDelete", "fa-trash", locale["itemDelete"]));
        }

        if (synchronizeFolder) {
            options.push(itemTmpl.format("cmfdSynch", "fa-refresh", locale["folderSynchronize"]));
            options.push(itemTmpl.format("cmfdSynchRecurse", "fa-refresh", locale["folderSynchronizeRecurse"]));
        }

        // Append menu options
        cmFolders.append(options);
        // Event delegation
        cmFolders.on("click", "li a", folderMenuHandler);
        // Create contextual menu
        cmFolders.puicontextmenu({
            target: deFolders
        });
    }

    // Build full url from relative path
    function buildURL(relativePath) {
        if (relativePath.length > 0 & relativePath[0] != "/") {
            relativePath = "/" + relativePath;
        }

        return location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '') + relativePath;
    }

    // Copy textual data to clipboard
    function copyToClipboard(text) {
        if (window.clipboardData && window.clipboardData.setData) {
            // IE specific code path to prevent textarea being shown while dialog is visible.
            return clipboardData.setData("Text", text);
        } else if (document.queryCommandSupported && document.queryCommandSupported("copy")) {
            var textarea = document.createElement("textarea");

            textarea.textContent = text;
            textarea.style.position = "fixed";  // Prevent scrolling to bottom of page in MS Edge.
            document.body.appendChild(textarea);
            textarea.select();
            try {
                return document.execCommand("copy");  // Security exception may be thrown by some browsers.
            } catch (ex) {
                console.warn(locale["clipboardFailed"], ex);
                return false;
            } finally {
                document.body.removeChild(textarea);
            }
        }
    }

    // File contextual menu event handler
    function fileMenuHandler(event) {
        var cmdName = event.currentTarget.id;
        var folder = selectedNode.data;

        switch (cmdName) {
            case "cmfSelectAll":
                deFiles.puidatatable("selectAllRows");
                break;
            case "cmfCut":
                var selection = deFiles.puidatatable("getSelection");

                command = new ItemCommand(folder.path, getFileNames(selection), true);
                break;
            case "cmfCopy":
                var selection = deFiles.puidatatable("getSelection");

                command = new ItemCommand(folder.path, getFileNames(selection));
                break;
            case "cmfPaste":
                if (command.path != folder.path) {
                    command.toPath = folder.path;
                    postCommand("PasteFiles", command).done(function (result, status, xhr) {
                        command = null;
                        deFolders.puitree("selectNode", selectedNode.node);
                    }).fail(function (xhr, result, status) {
                        showError(status, xhr.responseText);
                    });
                } else {
                    showMessage(locale["cantPaste"]);
                }
                break;
            case "cmfRename":
                var selection = deFiles.puidatatable("getContextMenuSelection");
                var cmd = new ItemCommand(folder.path, getFileNames(selection));
                var currentName = cmd.files[0];

                getName(locale["renameFileTitle"], currentName).then(function (newName) {
                    if (newName != currentName) {
                        cmd.toPath = newName;
                        postCommand("Rename", cmd).done(function (result, status, xhr) {
                            deFolders.puitree("selectNode", selectedNode.node);
                        }).fail(function (xhr, result, status) {
                            showError(status, xhr.responseText);
                        });
                    }
                });
                break;
            case "cmfDelete":
                showConfirm(locale["deleteFilesMessage"]).then(function () {
                    var selection = deFiles.puidatatable("getSelection");
                    var fileNames = getFileNames(selection);
                    var cmd = new ItemCommand(folder.path, fileNames);

                    postCommand("Delete", cmd).done(function (result, status, xhr) {
                        var datasource = deFiles.puidatatable('option', 'datasource');
                        var remaining = $.grep(datasource, function myfunction(file, index) {
                            return $.inArray(file.name, fileNames) == -1;
                        });

                        deFiles.puidatatable('option', 'datasource', remaining);
                    }).fail(function (xhr, result, status) {
                        showError(status, xhr.responseText);
                    });
                });
                break;
            case "cmfOpen":
                var file = deFiles.puidatatable("getContextMenuSelection");
                var cmd = new ItemCommand(folder.path, getFileNames(file));

                openFile(cmd);
                break;
            case "cmfDownload":
                var file = deFiles.puidatatable("getContextMenuSelection");
                var cmd = new ItemCommand(folder.path, getFileNames(file), true);

                getCommand("DownloadURL", cmd).done(function (result, status, xhr) {
                    dnn.dom.navigate(result);
                }).fail(function (xhr, result, status) {
                    showError(status, xhr.responseText);
                });
                break;
            case "cmfClipboard":
                var file = deFiles.puidatatable("getContextMenuSelection");
                var cmd = new ItemCommand(folder.path, getFileNames(file));

                getCommand("DownloadURL", cmd).done(function (result, status, xhr) {
                    var fullUrl = buildURL(result);

                    copyToClipboard(fullUrl);
                }).fail(function (xhr, result, status) {
                    showError(status, xhr.responseText);
                });
                break;
            case "cmfPack":
                getName(locale["packNameTitle"], "packedfiles").then(function (name) {
                    var selection = deFiles.puidatatable("getSelection");
                    var cmd = new ItemCommand(folder.path, getFileNames(selection), false, name);

                    postCommand("Pack", cmd).done(function (result, status, xhr) {
                        deFolders.puitree("selectNode", selectedNode.node);
                    }).fail(function (xhr, result, status) {
                        showError(status, xhr.responseText);
                    });
                });
                break;
            case "cmfUnpack":
                var selection = deFiles.puidatatable("getContextMenuSelection");
                var cmd = new ItemCommand(folder.path, getFileNames(selection));

                postCommand("Unpack", cmd).done(function (result, status, xhr) {
                    deFolders.puitree("selectNode", selectedNode.node);
                }).fail(function (xhr, result, status) {
                    showError(status, xhr.responseText);
                });
                break;
            default:
                break;
        }
    }

    // File contextual menu event handler
    function folderMenuHandler(event) {
        var cmdName = event.currentTarget.id;
        var folder = selectedNode.data;

        switch (cmdName) {
            case "cmfdCreate":
                getName(locale["createFolderTitle"], "").then(function (newName) {
                    var cmd = new ItemCommand(folder.path, null, false, newName);

                    postCommand("CreateFolder", cmd).done(function (result, status, xhr) {
                        deFolders.puitree("refresh");
                    }).fail(function (xhr, result, status) {
                        showError(status, xhr.responseText);
                    });
                });
                break;
            case "cmfdCut":
                command = new ItemCommand(folder.path);
                break;
            case "cmfdPaste":
                command.toPath = folder.path;
                postCommand("MoveFolder", command).done(function (result, status, xhr) {
                    command = null;
                    deFolders.puitree("refresh");
                }).fail(function (xhr, result, status) {
                    showError(status, xhr.responseText);
                });
                break;
            case "cmfdRename":
                var currentName = getFolderName(folder.path);

                getName(locale["renameFolderTitle"], currentName).then(function (newName) {
                    if (newName != currentName) {
                        var cmd = new ItemCommand(folder.path, null, false, newName);

                        postCommand("Rename", cmd).done(function (result, status, xhr) {
                            // Update folder path and save folder data
                            folder.path = folder.path.replace(/([^\/]+)\/$/g, newName + "/");
                            selectedNode.node.data('puidata', folder);
                            // Update node span label
                            selectedNode.node.find("span .ui-treenode-label").text(newName);
                        }).fail(function (xhr, result, status) {
                            showError(status, xhr.responseText);
                        });
                    }
                });
                break;
            case "cmfdDelete":
                showConfirm(locale["deleteFolderMessage"]).then(function () {
                    var cmd = new ItemCommand(folder.path);

                    postCommand("Delete", cmd).done(function (result, status, xhr) {
                        deFolders.puitree("refresh");
                    }).fail(function (xhr, result, status) {
                        showError(status, xhr.responseText);
                    });
                });
                break;
            case "cmfdSynch":
                var cmd = new ItemCommand(folder.path),
                    spinner = createSpinner($("#documentExplorer"));

                postCommand("Synchronize", cmd).done(function (result, status, xhr) {
                    deFolders.puitree("refresh");
                }).fail(function (xhr, result, status) {
                    showError(status, xhr.responseText);
                }).complete(function () {
                    spinner.remove();
                });
                break;
            case "cmfdSynchRecurse":
                var cmd = new ItemCommand(folder.path, null, true),
                    spinner = createSpinner($("#documentExplorer"));

                postCommand("Synchronize", cmd).done(function (result, status, xhr) {
                    deFolders.puitree("refresh");
                }).fail(function (xhr, result, status) {
                    showError(status, xhr.responseText);
                }).complete(function () {
                    spinner.remove();
                });
                break;
            default:
                break;
        }
    }

    // Get file url and open it in a new window
    function openFile(cmd) {
        getCommand("DownloadURL", cmd).done(function (result, status, xhr) {
            dnn.dom.navigate(result, "_new");
        }).fail(function (xhr, result, status) {
            showError(status, xhr.responseText);
        });
    }

    // Open selected file on double click
    function fileOpenHandler(event) {
        var folder = selectedNode.data;
        var file = deFiles.puidatatable("getSelection");
        var cmd = new ItemCommand(folder.path, getFileNames(file));

        openFile(cmd);
    }

    // Get folder name from path
    function getFolderName(path) {
        if (!path || path.length == 0)
            return "";

        var name = "";
        var pos = path.length - 1;

        // Remove trailing slash
        if (path.charAt(pos) == "/")
            name = path.substr(0, pos);

        pos = name.lastIndexOf("/");

        if (pos != -1)
            name = name.substr(pos - 1);

        return name;
    }

    // Folder node selection handler
    function folderSelectionHandler(event, node, data) {
        // Save current node folder
        selectedNode = node;

        // Manage options of the contextual menu
        var canManage = selectedNode.data.canManage;

        if (!canManage) {
            // Disable all links
            cmFolders.find("li a").each(function () {
                $(this).addClass("ui-state-disabled");
            });
        } else {
            var canPaste = canManage && command != null && command.files.length == 0;

            // Enable all links
            cmFolders.find("li a.ui-state-disabled").each(function () {
                $(this).removeClass("ui-state-disabled");
            });

            if (!canPaste)
                cmFolders.find("#cmfdPaste").addClass("ui-state-disabled");
        }

        // Retreive and display files
        getFiles(node.data.path);
    }

    // Manage options of the contextual file menu
    function fileSelectionHandler(event, data) {
        var isEmpty = data.name === undefined;
        var canManage = selectedNode.data.canManage;
        var canPaste = canManage && command != null && command.files.length > 0;

        // Disable all options if there is no file
        if (isEmpty) {
            // Disable all links
            cmFiles.find("li a").each(function () {
                $(this).addClass("ui-state-disabled");
            });
            // If can paste, enable this option
            if (canPaste) {
                cmFiles.find("#cmfPaste").removeClass("ui-state-disabled");
            }
            return;
        }

        var counter = deFiles.puidatatable("getSelection").length;
        var canUnpack = counter == 1 && data.extension == "zip";

        // Enable all links
        cmFiles.find("li a.ui-state-disabled").each(function () {
            $(this).removeClass("ui-state-disabled");
        });

        if (fileManagement) {
            if (!canPaste) {
                cmFiles.find("#cmfPaste").addClass("ui-state-disabled");
            }
            if (!canManage) {
                cmFiles.find("#cmfSelectAll").addClass("ui-state-disabled");
                cmFiles.find("#cmfCut").addClass("ui-state-disabled");
                cmFiles.find("#cmfRename").addClass("ui-state-disabled");
                cmFiles.find("#cmfDelete").addClass("ui-state-disabled");
                cmFiles.find("#cmfPack").addClass("ui-state-disabled");
                cmFiles.find("#cmfUnpack").addClass("ui-state-disabled");
            } else {
                if (!canUnpack) {
                    cmFiles.find("#cmfUnpack").addClass("ui-state-disabled");
                }
            }
        }

        if (counter > 1) {
            cmFiles.find("#cmfRename").addClass("ui-state-disabled");
            cmFiles.find("#cmfOpen").addClass("ui-state-disabled");
            cmFiles.find("#cmfDownload").addClass("ui-state-disabled");
            cmFiles.find("#cmfClipboard").addClass("ui-state-disabled");
        }
    }

    // Initialize interface
    function initInterface(options) {
        // Init local vars
        locale = options.resources;
        sizes = locale["sizes"].split(",");
        showIcon = options.showIcon;
        fileManagement = options.fileManagement;
        resetFilters = options.resetFilters;

        // Init dialog windows
        dlgWindow = $("#deDialogMessage");
        dlgMessage = $("#deDialogMessage > #message");
        dlgWindow.dialog({
            autoOpen: false,
            modal: true,
            closeOnEscape: true,
            minWidth: 350,
            dialogClass: "dnnFormPopup",
            title: locale["errorTitle"],
            buttons: [
                {
                    text: locale["dialogOk"],
                    click: function () {
                        $(this).dialog("close");
                    }
                }
            ]
        });

        // Init files data table
        deFiles = $("#deFiles");
        deFiles.puidatatable({
            paginator: {
                rows: options.rows
            },
            columns: options.columns,
            draggableColumns: true,
            resizableColumns: true,
            selectionMode: "multiple",
            rowSelectContextMenu: fileSelectionHandler,
            datasource: [],
            emptyMessage: locale["noFiles"]
        });
        if (options.openOnDblclick)
            deFiles.on("dblclick", ".ui-datatable-data tr", fileOpenHandler);

        // Define files contextual menu object
        cmFiles = $("#cmFiles");
        buildFileMenu();

        // Init folder tree
        deFolders = $("#deFolders");
        deFolders.puitree({
            animate: true,
            lazy: true,
            selectionMode: "single",
            nodeSelect: folderSelectionHandler,
            nodes: getFolders,
            icons: {
                def: {
                    expanded: "fa-folder-open",
                    collapsed: "fa-folder"
                }
            }
        });

        // Define folders contextual menu object
        cmFolders = $("#cmFolders");

        if (fileManagement) {
            builFolderMenu(options.synchronizeFolder);
        } else {
            cmFolders.remove();
        }
    }

    // Module init function
    de.init = function (dnnSF) {
        // Init DNN Service Framework
        sf = dnnSF;
        services = sf.getServiceRoot("TidyModules/DocumentExplorer") + "Services/{0}/";

        // Define string format function
        if (!String.prototype.format) {
            String.prototype.format = function () {
                var args = arguments;

                return this.replace(/{(\d+)}/g, function (match, number) {
                    return typeof args[number] != "undefined" ? args[number] : match;
                });
            }
        }

        // Extend jQuery UI Dialog to manage enter key as default button
        $.extend($.ui.dialog.prototype.options, {
            create: function () {
                var $this = $(this);

                // focus first button and bind enter to it
                $this.parent().find('.ui-dialog-buttonpane button:first').focus();
                $this.keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        $this.parent().find('.ui-dialog-buttonpane button:first').click();

                        return false;
                    }
                });
            }
        });

        // Extend PrimeUI Tree widget with a 'refresh' method to rebuild content
        $.widget("primeui.puitree", $.primeui.puitree, {
            refresh: function () {
                this.rootContainer.empty();

                if (this.options.selectionMode) {
                    this.selection = [];
                }

                if ($.type(this.options.nodes) === 'array') {
                    this._renderNodes(this.options.nodes, this.rootContainer);
                }
                else if ($.type(this.options.nodes) === 'function') {
                    this.options.nodes.call(this, {}, this._initData);
                }
            }
        });

        // Extend PrimeUI DataTable widget with a 'selectAllRows' method
        $.widget("primeui.puidatatable", $.primeui.puidatatable, {
            selectAllRows: function () {
                if (this.options.selectionMode) {
                    this.selection = this.data;
                    this.paginate();
                }
            }
        });

        // Get options then initialize user interface
        getCommand("Options").done(function (options, status, xhr) {
            // Map column formaters
            var columns = options.columns;

            for (var i = 0; i < columns.length; i++) {

                if (columns[i].filter && !hasFilter)
                    hasFilter = true;

                switch (columns[i].field) {
                    case "name":
                        columns[i].content = options.showIcon ? composeName : null;
                        break;
                    case "size":
                        columns[i].content = bytesToSize;
                        break;
                    case "modified":
                        columns[i].content = formatDate;
                        break;
                    default:
                        break;
                }
            }

            // Initialize interface
            initInterface(options);
        }).fail(function (xhr, result, status) {
            var message = "{0}: {1}";

            alert(message.format(result, status));
        });
    }

}(window.tm_de = window.tm_de || {}, jQuery));
