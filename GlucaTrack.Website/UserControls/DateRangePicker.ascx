<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DateRangePicker.ascx.cs" Inherits="GlucaTrack.Website.DateRangePicker" %>
<script src="/jQuery/Content/Scripts/jquery-1.7.1.min.js" type="text/javascript"></script>
<script type="text/javascript">
    function pageLoad(sender, args) {
        if (args.get_isPartialLoad()) {
            var rangeType = $('#<%= DateRangeList.ClientID%>').val();
            if (rangeType == 6) {
                $('#DatePeriodRow').removeAttr("style");
            }
            else {
                $('#DatePeriodRow').attr("style", "display: none;");
            }
        }
    }

    $(document).ready(function () {
        $('#<%= DateRangeList.ClientID %>').live("change", function () {
            var rangeType = $('#<%= DateRangeList.ClientID %>').val();
            if (rangeType == 6) {
                $('#DatePeriodRow').removeAttr("style");
            }
            else {
                $('#DatePeriodRow').attr("style", "display: none;");
            }
        });

        $('#<%= DateRangeList.ClientID %>').live("click", function (event) {
            $('#<%= DateRangeList.ClientID %>').change();
        });

        $('#<%= DateRangeList.ClientID %>').live("blur", function () {
            $('#<%= DateRangeList.ClientID %>').change();
        });
    });
</script>
<table id="DateRangePickerTable">
    <tr>
        <td id="Td1" runat="server">
            <asp:Label ID="PeriodLabel" runat="server" Text="Period" />
        </td>
        <td id="Td2" colspan="7" class="TdStyle" runat="server">
            <asp:DropDownList ID="DateRangeList" runat="server" Width="100px" OnSelectedIndexChanged="DateRangeList_SelectedIndexChanged">
               <asp:ListItem Text="Last 7 Days" Value="2" Selected="True"/>
               <asp:ListItem Text="Last 14 Days" Value="3" />
               <asp:ListItem Text="Last 30 Days" Value="4"/>
               <asp:ListItem Text="Last 60 Days" Value="5"/>
               <asp:ListItem Text="Yesterday" Value="1"/>
               <asp:ListItem Text="Today" Value="0"/>
               <asp:ListItem Text="Period" Value="6"/>
            </asp:DropDownList>
        </td>
    </tr>
    <tr id="DatePeriodRow"style="display: none;">
        <td>
            <asp:Label ID="FromLabel" runat="server" Text="From Date" />
        </td>
        <td style="width: 108px">
            <asp:TextBox ID="FromDateTextBox" runat="server" Width="100px" OnTextChanged="FromDateTextBox_TextChanged"></asp:TextBox>
            <ajaxToolkit:CalendarExtender ID="FromDateCalendarExtender" runat="server" Enabled="True"
               PopupButtonID="FromDateImageButton" TargetControlID="FromDateTextBox" />
            <ajaxToolkit:MaskedEditExtender ID="FromDateMaskedEditExtender" runat="server" CultureName="en-US"
               Enabled="True" Mask="99/99/9999" MaskType="Date" TargetControlID="FromDateTextBox" />
        </td>
        <td style="height: 20px; padding-top: 5px; width: 50px;">
            <asp:ImageButton ID="FromDateImageButton" runat="server" ImageUrl="/Images/icons/calendar.png"
               CausesValidation="False" ImageAlign="Baseline"/>
        </td>
        <td id="Td5" runat="server">
            <ajaxToolkit:MaskedEditValidator ID="FromDateMaskedEditValidator" runat="server"
               Display="Dynamic" ControlExtender="FromDateMaskedEditExtender" ControlToValidate="FromDateTextBox"
               EmptyValueMessage="Required" ErrorMessage="FromDateMaskedEditValidator" InvalidValueMessage="Invalid Date"
               SetFocusOnError="True" IsValidEmpty="False" />
        </td>
        <td>
            <asp:Label ID="ToLabel" runat="server" Text="To Date" />
        </td>
        <td style="width: 108px">
            <asp:TextBox ID="ToDateTextBox" runat="server" Width="100px" OnTextChanged="ToDateTextBox_TextChanged" />
            <ajaxToolkit:CalendarExtender ID="ToDateCalendarExtender" runat="server" Enabled="True"
               PopupButtonID="ToDateImageButton" TargetControlID="ToDateTextBox" />
            <ajaxToolkit:MaskedEditExtender ID="ToDateMaskedEditExtender" runat="server" CultureName="en-US"
               Enabled="True" Mask="99/99/9999" MaskType="Date" TargetControlID="ToDateTextBox" />
        </td>
        <td style="height: 20px; padding-top: 5px; width: 50px;">
            <asp:ImageButton ID="ToDateImageButton" runat="server" ImageUrl="/Images/icons/calendar.png"
               CausesValidation="False" ImageAlign="Baseline"/>
        </td>
        <td>
            <ajaxToolkit:MaskedEditValidator ID="ToDateMaskedEditValidator" runat="server" Display="Dynamic"
               ControlExtender="ToDateMaskedEditExtender" ControlToValidate="ToDateTextBox"
               EmptyValueMessage="Required" ErrorMessage="ToDateMaskedEditValidator" InvalidValueMessage="Invalid Date"
               SetFocusOnError="True" IsValidEmpty="False" />
        </td>
    </tr>
</table>
