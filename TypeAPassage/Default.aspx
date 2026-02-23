<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TypeAPassage.Home" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" href="/App_Themes/TypeAPassageTheme.css" />
    <title>Type A Passage</title>
    <script src="Scripts/jquery-3.6.0.min.js"></script>

    <script type="text/javascript">
        let totalWords;
        let correctWords = new Array();
        let accuracy;
        let currentIndex = 0;
        let backspaceIndex = 0;
        let displayBookValue;
        let userTextValue;
        function countdown() {
            displayBookBox = $("#BookLinkBox");
            userTextBox = $("#TypingTestBox");
            bookLinkLabel = $("#BookLinkLabel");
            startTestButton = $("#StartTestButton");
            stopTestButton = $("#StopTestButton");
            currentIndexLabel = $("CurrentIndexLabel");
            displayBookValue = displayBookBox.val();
            var i = 0; //Starting index
            var seconds = document.getElementById("TimerLabel").innerHTML;
            displayBookValue = displayBookValue.split(" ");
            totalWords = <%=wordCount%>;
            $(userTextBox).keyup(function (e) {
                userTextValue = userTextBox.val();
                userTextValue = userTextValue.split(" ");
                if (userTextValue[i] == displayBookValue[i] && e.keyCode == 32) {//32 = space bar
                    $(userTextBox).css('color', 'green');
                    i++;
                    currentIndex++;
                    currentIndexLabel.innerHTML = currentIndex.val.toString();
                }
                else if (userTextValue[i] != displayBookValue[i] && e.keyCode == 32) {
                    $(userTextBox).css('color', 'red');
                    i++;
                    currentIndex++;
                    currentIndexLabel.innerHTML = currentIndex.val.toString();
                }
                else if (e.keyCode == 8 && i > userTextValue.length) {//8 = backspace
                    i--;
                    backspaceIndex--;
                    currentIndexLabel.innerHTML = currentIndex.val.toString();
                }
                if (userTextValue.length == displayBookValue.length) {
                    for (var j = 0; j < displayBookValue.length; j++) {
                        if (userTextValue[j] == displayBookValue[j]) {
                            correctWords.push(userTextValue[j]);
                        }
                    }
                    accuracy = (correctWords.length / totalWords) * 100.00;
                    alert("Total Words: " + totalWords + "\n" +
                        "Correct Words: " + correctWords.length + "\n" +
                        "Accuracy: " + accuracy.toFixed(2) + "%" + "\n" +
                        "Words per Minute: " + ((correctWords.length / 60) * 100).toFixed(2));
                }
            });

            if (seconds > 0) {
                document.getElementById("TimerLabel").innerHTML = seconds - 1;
                setTimeout("countdown()", 1000);
            }
            if (seconds == 0/* || document.getElementById('StopTestButton').clicked == true*/) {
                for (var j = 0; j < displayBookValue.length; j++) {
                    if (userTextValue[j] == displayBookValue[j]) {
                        correctWords.push(userTextValue[j]);
                    }
                }
                accuracy = (correctWords.length / totalWords) * 100.00;
                alert("Total Words: " + totalWords + "\n" +
                    "Correct Words: " + correctWords.length + "\n" +
                    "Accuracy: " + accuracy.toFixed(2) + "%" + "\n" +
                    "Words per Minute: " + ((correctWords.length / 60) * 100).toFixed(2));
            }
        }
    </script>
</head>
<body>
    <div id="MainContainer">
        <form id="form1" runat="server">
            <div id="BookLinkDiv">
                <asp:Label ID="BookLinkLabel" runat="server"></asp:Label>
                <asp:ScriptManager ID="TypeAPassageScriptManager" runat="server" EnablePageMethods="true"></asp:ScriptManager>
                <div id="SelectedBookDiv">
                    <asp:TextBox ID="BookLinkBox" runat="server" ReadOnly="true" Wrap="true" TextMode="MultiLine" CssClass="BookLinkBoxStyle"></asp:TextBox>
                </div>
            </div>
            <br />
            <div id="TypeTextDiv">
                <asp:UpdatePanel ID="TypingTestAreaPanel" runat="server" ChildrenAsTriggers="true">
                    <ContentTemplate>
                        <asp:Label ID="TypingTextLabel" runat="server" Text="Type Here:"></asp:Label>
                        <asp:TextBox ID="TypingTestBox" runat="server" Wrap="true" TextMode="MultiLine" CssClass="BookLinkBoxStyle" Enabled="false"></asp:TextBox>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div id="TypingTestButtons">
                <asp:UpdatePanel ID="TypingTestButtonPanel" runat="server" ChildrenAsTriggers="true">
                    <ContentTemplate>
                        <asp:Button ID="StartTestButton" ClientIDMode="Static" CssClass="button" runat="server" Text="Begin Test" OnClick="StartTestButton_Click" OnClientClick="countdown()" />
                        <asp:Button ID="StopTestButton" ClientIDMode="Static" CssClass="button" runat="server" Text="Stop Test" />
                        <asp:Label ID="TimerLabel" runat="server">60</asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </form>
    </div>
</body>
</html>
