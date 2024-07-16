<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DistPrdMap.aspx.cs" Inherits="DistPrdMap_UpdStck.DistPrdMap" Async="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Distributor's Product Mapping</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="Content/bootstrap.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.9.0/css/bootstrap-datepicker.min.css" />

    <style>
        .toast-custom {
            position: fixed;
            top: 10px;
            right: 20px;
            width: 300px;
            background-color: #fff;
            border-radius: 5px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            padding: 15px;
        }

        .toast-success {
            border-left: 5px solid #28a745; /* Light green */
        }

        .toast-danger {
            border-left: 5px solid #dc3545; /* Red */
        }

        .toast-partial {
            border-left: 5px solid #ff6a00; /* Orange */
        }

        .form-control {
            text-align: center;
        }

        @media (max-width: 768px) {
            .col-12 {
                text-align: center;
            }

            #btnOpenModal {
                width: 100%;
            }
        }

        .navbar-white .navbar-toggler {
            border-color: rgba(0, 0, 0, 0.1);
        }

        .navbar-white .navbar-toggler-icon {
            background-image: url("data:image/svg+xml;charset=utf8,%3Csvg viewBox='0 0 30 30' xmlns='http://www.w3.org/2000/svg'%3E%3Cpath stroke='rgba(0, 0, 0, 0.5)' stroke-width='2' stroke-linecap='round' stroke-miterlimit='10' d='M4 7h22M4 15h22M4 23h22'/%3E%3C/svg%3E");
        }
    </style>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.bundle.min.js"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.6.0/jquery.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.29.1/moment.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.9.0/js/bootstrap-datepicker.min.js"></script>
    <script>
        function showToast(message, styleClass) {
            var toast = $('<div class="toast-custom ' + styleClass + '">' + message + '</div>').appendTo('#toastContainer');

            // Show the toast
            toast.fadeIn();

            // Move existing toasts down
            $('.toast-custom').not(toast).each(function () {
                $(this).animate({ top: "+=" + (toast.outerHeight() + 10) }, 'fast');
            });

            // Hide the toast after 3 seconds
            setTimeout(function () {
                toast.fadeOut(function () {
                    // Remove the toast from DOM after fadeOut
                    $(this).remove();

                    // Move remaining toasts up
                    $('.toast-custom').each(function (index) {
                        $(this).animate({ top: "-=" + (toast.outerHeight() + 10) }, 'fast');
                    });
                });
            }, 6000);
        }
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <nav class="navbar navbar-expand-lg navbar-white bg-white">
            <div class="container">
                <a class="navbar-brand" runat="server" href="~/Home">SYNAPSE</a>
                <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="collapse navbar-collapse" id="navbarNav">
                    <ul class="navbar-nav ml-auto">
                        <li class="nav-item"><a class="nav-link" runat="server" href="~/DistPrdMap">Distributor'sProductMapping</a></li>
                        <li class="nav-item"><a class="nav-link" runat="server" href="~/UpdateStock">UpdateStock</a></li>
                        <li class="nav-item"><a class="nav-link" runat="server" href="~/">LogOut</a></li>
                    </ul>
                </div>
            </div>
        </nav>
        <hr />

        <div class="container body-content">
            <div class="headtag">
                <asp:Label ID="lblUserName" runat="server" Style="color: black; float: right; margin-top: 0px; margin-bottom: -20px; margin-right: 20px"></asp:Label>
            </div>
            <table style="width: 100%; font-family: Calibri; font-size: small">
                <tr>
                    <td style="text-align: right">
                        <asp:Label ID="lbl_msg" Text="" runat="server" ForeColor="Red" Font-Bold="true" Font-Size="Large" BackColor="LightPink"></asp:Label>
                    </td>
                </tr>
            </table>

            <h2 style="text-align: center; margin-top: 20px;">Distributor's Product Mapping</h2>

            <br />

            <div class="container">
                <div class="row">
                    <div class="col-12 col-md-2 mb-2 mb-md-0">
                        <asp:DropDownList ID="DistributorErpId" runat="server" AutoPostBack="true" class="form-control" OnSelectedIndexChanged="DistributorErpId_SelectedIndexChanged">
                            <asp:ListItem Text="DistributorErpId" Value="DistributorErpIdValue"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-12 col-md-2 mb-2 mb-md-0">
                        <button type="button" class="form-control" id="btnOpenModal" data-toggle="modal" data-target="#exampleModalCenter">
                            Product ErpId
                        </button>
                    </div>
                    <div class="col-12 col-md-2 mb-2 mb-md-0">
                        <asp:DropDownList ID="ActionDrp" runat="server" AutoPostBack="true" class="form-control" OnSelectedIndexChanged="ActionDrp_SelectedIndexChanged">
                            <asp:ListItem Text="Action Type" Value=""></asp:ListItem>
                            <asp:ListItem Text="Attach" Value="Attach"></asp:ListItem>
                            <asp:ListItem Text="Detach" Value="Detach"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-12 col-md-2 mb-2 mb-md-0">
                        <asp:Button ID="Add" runat="server" Text="Add" CssClass="btn btn-primary form-control" OnClick="btnAdd_Click" />
                    </div>
                    <div class="col-12 col-md-2 mb-2 mb-md-0">
                        <asp:Button ID="Submit" runat="server" Text="Submit" CssClass="btn btn-success form-control" OnClick="btnSubmit_Click" />
                    </div>
                    <div class="col-12 mt-2">
                        <asp:HiddenField ID="hiddenSelectedMappings" runat="server" />
                    </div>
                </div>
            </div>

            <%-- Modal for DistCode --%>
            <div class="modal fade" id="exampleModalCenter" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="exampleModalLongTitle">Product ErpId</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body" style="max-height: 400px; overflow-y: auto;">
                            <div class="form-group">
                                <input type="text" id="txtSearch" class="form-control" placeholder="Search..." />
                            </div>
                            <div class="form-group">
                                <asp:GridView ID="PrdErpId" runat="server" AutoPostBack="True" CssClass="table table-bordered form-group"
                                    AutoGenerateColumns="false" DataKeyNames="PrdNm" Style="width: 465px; margin-top: 10px; margin-bottom: -40px; text-align: center"
                                    ShowHeader="false" OnRowDataBound="PrdErpId_RowDataBound">
                                    <Columns>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <div style="margin-right: 10px;">
                                                    <input type="checkbox" id="CheckBox1" runat="server" class="form-check-input" style="margin-left: -3px;" />
                                                </div>
                                                <asp:HiddenField ID="HiddenPrdErpId" runat="server" Value='<%# Eval("PrdCode") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="PrdNm" />
                                    </Columns>
                                    <HeaderStyle CssClass="header-hidden" />
                                    <RowStyle CssClass="fixed-height-row" BackColor="#FFFFFF" />
                                </asp:GridView>
                            </div>
                        </div>

                        <div class="modal-footer">
                            <button type="button" class="btn btn-primary" onclick="selectItems()" data-dismiss="modal">Select</button>
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>

            <%-- Notification Label --%>
            <div id="toastContainer" aria-live="polite" aria-atomic="true" style="position: relative; min-height: 200px;"></div>
            <asp:HiddenField ID="hdnBusinessType" runat="server" />
            <asp:HiddenField ID="hdnRole" runat="server" />
        </div>
    </form>

    <%-- Script for search button in Modal --%>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#txtSearch").on("keyup", function () {
                var value = $(this).val().toLowerCase();
                $("#<%= PrdErpId.ClientID %> tr").filter(function () {
                    $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
                });
            });
        });
    </script>

    <%-- Script for datepicker --%>
    <script>
        $(document).ready(function () {
            $('.input-group.date').datepicker({
                format: 'dd-mm-yyyy',
                startDate: 'today',
                autoclose: true
            });
        });
    </script>
</body>
</html>
