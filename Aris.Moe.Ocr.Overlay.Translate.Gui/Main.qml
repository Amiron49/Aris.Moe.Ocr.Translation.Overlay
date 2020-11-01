import QtQuick 2.9
import QtQuick.Layouts 1.3
import QtQuick.Controls 2.3
import QtQuick.Controls.Material 2.1
import Aris.Moe.Ocr.Overlay.Translate.Gui 1.1

ApplicationWindow {
    id: window
    visible: true
    width: 530
    height: 140
    title: qsTr("Aris.Moe Translation Overlay")
    
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
//            MenuItem {
//                text: qsTr("Settings")
//                onTriggered: {
//                    stackView.push("Pages/Settings.qml")
//                }
//            }
        }
    }
    //Content Area
    
    ColumnLayout{            
        StackView {
            id: stackView
    
            initialItem: "Pages/Controls.qml"
        }
        
    }
}