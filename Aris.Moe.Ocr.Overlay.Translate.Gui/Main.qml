import QtQuick 2.9
import QtQuick.Layouts 1.3
import QtQuick.Controls 2.3
import QtQuick.Controls.Material 2.1

ApplicationWindow {
    id: window
    visible: true
    width: 640
    height: 480
    title: qsTr("Hello World")
    
    menuBar: MenuBar {
        Menu {
            title: qsTr("File")
            MenuItem {
                text: qsTr("&Open")
                onTriggered: console.log("Open action triggered");
            }
            MenuItem {
                text: qsTr("Exit")
                onTriggered: Qt.quit();
            }
            MenuItem {
                text: qsTr("Settings")
                onTriggered: {
                    stackView.push("Pages/Settings.qml")
                }
            }
        }
    }
    //Content Area
    
    ColumnLayout{
 
    
        Button {
            text: 'Back'
            onClicked: {
                stackView.pop()
            }
        }
            
        StackView {
            id: stackView
    
            initialItem: "Pages/Controls.qml"
        }
    }
}