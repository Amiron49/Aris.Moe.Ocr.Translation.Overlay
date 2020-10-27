ScrollablePage {
    Column {
        Label { text: qsTr("Input") }

        Grid {
            id: inputContainer
            columns: 2
            spacing: 12
            width: parent.width
        
            TextField { 
                id: txtValue1
                placeholderText: qsTr("Value 1")

                onEditingFinished: {
                        inputContainer.validateInput()
                }
            }
            TextField { 
                id: txtValue2
                placeholderText: qsTr("Value 2")

                onEditingFinished: {
                        inputContainer.validateInput()
                }
            }

            Label { 
                id: lblValidation
                color: 'red'
            }
        }
    }
}