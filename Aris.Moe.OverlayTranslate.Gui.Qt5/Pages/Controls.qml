﻿import QtQuick 2.6
import QtQuick.Controls 2.1
import Aris.Moe.OverlayTranslate.Gui.Qt5 1.1

ScrollablePage {
    Column {
        width: parent.width
        Row {
            spacing: 12
            
            Button {
                text: 'Translate'
                onClicked: {
                    var task = model.translateScreen()
                    Net.await(task, function() {
                    })
                }
            }
            
            Button {
                text: 'Hide Overlay'
                onClicked: {
                    model.hideOverlay()
                }
            }
            
            Button {
                text: 'Show Overlay'
                onClicked: {
                    model.showOverlay()
                }
            }
            
            Button {
                text: 'Change Target area'
                onClicked: {
                    model.askForTargetResize()
                }
            }
            
            Button {
                text: 'OCR only'
                 onClicked: {
                    var task = model.ocrScreen()
                    Net.await(task, function() {
                    })
                }
            }
            
            Button {
                text: 'Progress'
                 onClicked: {
                    var task = model.displayProgress()
                }
            }
        
            ControlsModel {
                id: model
            }
        }
        
        Text {
            font.pointSize: 9
            textFormat: Text.PlainText
            text: model.getErrors()
        }
    }
}