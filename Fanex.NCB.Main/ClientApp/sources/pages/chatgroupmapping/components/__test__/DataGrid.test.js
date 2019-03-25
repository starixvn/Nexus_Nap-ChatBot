import React from 'react';
import { DataGrid } from '../DataGrid';
import renderer from 'react-test-renderer';
import ShallowRenderer from 'react-test-renderer/shallow';

describe("DataGrid", () => { 
    describe("render", () => { 
        it("when gridData null - return null", () => { 
            // Arrange
            let props = { gridData: null};
            let dataGrid = new DataGrid(props);

            // Act
            let result = dataGrid.render();

            // Assert
            expect(result).toBe(null);
        })

        it("when gridData.data null - return null", () => {
            // Arrange
            let props = {
                gridData: {data: null} };
            let dataGrid = new DataGrid(props);

            // Act
            let result = dataGrid.render();

            // Assert
            expect(result).toBe(null);
        })

        it("when has data", () => { 
            // Arrange
            let props = {
                gridData: {
                    data: [
                        {
                            botName: "name 1",
                            conversationID: 1,
                            groupName: "name 2",
                            recipientTypeName: "name 3",
                            recipientNames: "name 4",
                            recipientStatusNames: "name 5",
                            mappingStatusID: 1,
                            mappingStatusName: "name 6",
                            remark: "name 7"
                        },
                        {
                            botName: "name 11",
                            conversationID: 11,
                            groupName: "name 21",
                            recipientTypeName: "name 31",
                            recipientNames: "name 41",
                            recipientStatusNames: "name 51",
                            mappingStatusID: 11,
                            mappingStatusName: "name 61",
                            remark: "name 71"
                        }
                    ],
                    count: 2
                },
                gridParams: {
                    pageIndex: 1,
                    pageSize: 10
                }
            };

            // Act
            let dataGrid = (new ShallowRenderer()).render(<DataGrid {...props} />);

            // Assert
            expect(dataGrid).toMatchSnapshot();
        })
    })

    describe("onPaging", () => {
        it("when paging - set params and get data", () => {
            // Arrange
            let pageIndex = 2;
            let pageSize = 10;              
            let props = {
                gridParams: {},
                setGridParams: jest.fn(),
                getGridData: jest.fn()
            };
            let dataGrid = new DataGrid(props);
            let expectedParam = { pageIndex: 2, pageSize: 10};

            // Act
            dataGrid.onPaging(pageIndex, pageSize);

            // Assert
            expect(props.setGridParams).toHaveBeenCalledWith(expectedParam);
            expect(props.getGridData).toHaveBeenCalledWith(expectedParam);
        })
    })

    describe("openEditPopup", () => {
        it("filter recipients and open edit poppup", () => {
            // Arrange 
            let item = {
                recipientTypeID: 12
            };
            let props = {
                filterRecipients: jest.fn(),
                openEditPopup: jest.fn(),
                settingData: {
                    recipients: "recipients"
                }
            };
            let dataGrid = new DataGrid(props);

            // Act
            dataGrid.openEditPopup(item);

            // Assert
            expect(props.filterRecipients).toHaveBeenCalledWith("recipients", 12);
            expect(props.openEditPopup).toHaveBeenCalledWith(item);
        })     
    })
})