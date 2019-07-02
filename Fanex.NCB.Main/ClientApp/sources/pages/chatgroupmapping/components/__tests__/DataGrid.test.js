import React from 'react';
import { DataGrid } from '../DataGrid';
import renderer from 'react-test-renderer';
import ShallowRenderer from 'react-test-renderer/shallow';
import CommonFunctions from '../../../common/CommonFunctions';

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
        });

        it("when gridData.data null - return null", () => {
            // Arrange
            let props = {
                gridData: {data: null} };
            let dataGrid = new DataGrid(props);

            // Act
            let result = dataGrid.render();

            // Assert
            expect(result).toBe(null);
        });

        it("when gridData.data empty - render NoDataTitle", () => {
            // Arrange
            let props = {
                gridData: {
                    data: [],
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
        });

        it("when has data - render table", () => { 
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
        });
    });

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
        });
    });

    describe("openEditPopup", () => {
        it("when isBotRemoved status - do not call filterRecipients and openEditPopup", () => {
            // Arrange
            let item = {};
            let props = {
                filterRecipients: jest.fn(),
                openEditPopup: jest.fn()
            };
            let dataGrid = new DataGrid(props);

            let isBotRemoved = jest.fn(() => { return true; });
            CommonFunctions.isBotRemoved = isBotRemoved.bind(CommonFunctions);

            // Act
            dataGrid.openEditPopup(item);

            // Assert
            expect(props.filterRecipients).not.toHaveBeenCalled();
            expect(props.openEditPopup).not.toHaveBeenCalled();
        });

        it("when NOT isBotRemoved status - filter recipients and open edit poppup", () => {
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

            let isBotRemoved = jest.fn(() => { return false; });
            CommonFunctions.isBotRemoved = isBotRemoved.bind(CommonFunctions);

            // Act
            dataGrid.openEditPopup(item);

            // Assert
            expect(props.filterRecipients).toHaveBeenCalledWith("recipients", 12);
            expect(props.openEditPopup).toHaveBeenCalledWith(item);
        });     
    });

    describe("getLicenseeStatus", () => {
        it("when recipientTypeName null or empty - return empty", () => {
            // Arrange
            let item = {
                recipientTypeName: null
            };
            let dataGrid = new DataGrid();

            // Act
            var result = dataGrid.getLicenseeStatus(item);

            // Assert
            expect(result).toEqual("");
        });

        it("when recipientTypeName NOT null or empty - call CommonFunctions.getLicenseeStatus", () => {
            // Arrange
            let item = {
                recipientTypeName: "something",
                recipientStatusNames: "recipient Status Names"
            };
            let dataGrid = new DataGrid();

            CommonFunctions.getLicenseeStatus = jest.fn();

            // Act
            dataGrid.getLicenseeStatus(item);

            // Assert
            expect(CommonFunctions.getLicenseeStatus).toHaveBeenCalledWith(item.recipientStatusNames);
        });
    });

    describe("getEditClassNames ", () => {
        it("when isBotRemoved - return className with edit-disabled", () => {
            // Arrange
            let item = {};
            let dataGrid = new DataGrid();

            let isBotRemoved = jest.fn(() => { return true; });
            CommonFunctions.isBotRemoved = isBotRemoved.bind(CommonFunctions);

            // Act
            var result = dataGrid.getEditClassNames(item);

            // Assert
            expect(result).toEqual("icon-edit edit-disabled");
        });

        it("when NOT isBotRemoved - return className without edit-disabled", () => {
            // Arrange
            let item = {};
            let dataGrid = new DataGrid();

            let isBotRemoved = jest.fn(() => { return false; });
            CommonFunctions.isBotRemoved = isBotRemoved.bind(CommonFunctions);

            // Act
            var result = dataGrid.getEditClassNames(item);

            // Assert
            expect(result).toEqual("icon-edit");
        });
    });
});