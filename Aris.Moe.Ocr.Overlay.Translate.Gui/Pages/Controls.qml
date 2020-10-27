import QtQuick 2.6
import QtQuick.Controls 2.1
import Aris.Moe.Ocr.Overlay.Translate.Gui 1.1

ScrollablePage {
    Grid {
        columns: 6
        spacing: 12
        width: parent.width
    
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
        ControlsModel {
            id: model
        }
    }
}