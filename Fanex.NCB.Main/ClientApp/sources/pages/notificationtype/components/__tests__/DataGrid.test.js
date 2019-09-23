import React from 'react';
import {DataGrid} from '../DataGrid';
import renderer from 'react-test-renderer';
import ShallowRenderer from 'react-test-renderer/shallow'; 
import Adapter from 'enzyme-adapter-react-16';
import configureMockStore from 'redux-mock-store';
import { shallow, render, mount, configure } from 'enzyme';

describe('DataGrid', () => {   
    describe('mount', () => {
        it('when componentWillMount - setGridParams, getGridData', () => {
            // Arrange
            let props = {                
                setGridParams: jest.fn(),
                getGridData: jest.fn()                
            };  
            let params = {
                pageIndex: 1, 
                pageSize: 10
            };
            let dataGrid = new DataGrid(props); 

            // Act
            dataGrid.componentWillMount();

            // Assert
            expect(props.setGridParams).toHaveBeenCalledWith(params);
            expect(props.getGridData).toHaveBeenCalledWith(params);
        });
    });

    describe("onPaging", () => {
        it("when onPaging - setGridParams, getGridData", () => {
            // Arrange
            let props = {                
                setGridParams: jest.fn(),
                getGridData: jest.fn()                
            };  
            const params = {
                pageIndex: 2, 
                pageSize: 20
            };
            let dataGrid = new DataGrid(props);

            // Act
            dataGrid.onPaging(params.pageIndex, params.pageSize);

            // Assert
            expect(props.setGridParams).toHaveBeenCalledWith(params);
            expect(props.getGridData).toHaveBeenCalledWith(params);
        });
    });

    describe("openEditPopup", () => {
        it("when openEditPopup - updateActiveRowIndex, openEditPopup", () => {
            // Arrange 
            let props = {
                updateActiveRowIndex: jest.fn(),
                openEditPopup: jest.fn()
            };
            const item = {data: "some data"};
            const index = 3;
            let dataGrid = new DataGrid(props);

            // Act
            dataGrid.openEditPopup(item, index);

            // Assert
            expect(props.updateActiveRowIndex).toHaveBeenCalledWith(index);
            expect(props.openEditPopup).toHaveBeenCalledWith(item);
        });
    });

    describe("openDeletePopup", () => {
        it("when openDeletePopup - updateActiveRowIndex, openDeletePopup", () => {
            // Arrange 
            let props = {
                updateActiveRowIndex: jest.fn(),
                openDeletePopup: jest.fn()
            };
            let notificationTypeId = 1;
            const notificationTypeName = "some data";
            const index = 3;
            let dataGrid = new DataGrid(props);

            // Act
            dataGrid.openDeletePopup(index, notificationTypeId, notificationTypeName);

            // Assert
            expect(props.updateActiveRowIndex).toHaveBeenCalledWith(index);
            expect(props.openDeletePopup).toHaveBeenCalledWith(notificationTypeId, notificationTypeName);
        });
    });    

    describe("render", () => {
        const mockStore = configureMockStore();
        configure({ adapter: new Adapter() });

        it("when no data - return null", () => {
            // Arrange
            let props = {
                gridData: null
            };
            let dataGrid = new DataGrid(props);

            // Act
            let result = dataGrid.render();

            // Assert
            expect(result).toBe(null);
        });

        it("when notificationTypes null - return null", () => {
            // Arrange
            let props = {
                gridData: { notificationTypes: null}
            };
            let dataGrid = new DataGrid(props);

            // Act
            let result = dataGrid.render();

            // Assert
            expect(result).toBe(null);
        });

        it("when has data", () => {
            // Arrange
            const data = [
                {
                    notificationTypeName: "notificationTypeName 1",
                    recipientTypeName: "recipientTypeName 1",
                    systemName: "systemName 1",
                    recipientTypes: [{ name: "a" }, { name: "b" }],
                    chatGroupTypes: [{ name: "a" }, { name: "b" }]
                },
                {
                    notificationTypeName: "notificationTypeName 2",
                    recipientTypeName: "recipientTypeName 2",
                    systemName: "systemName 2",
                    recipientTypes: [{ name: "a" }, { name: "b" }],
                    chatGroupTypes: [{ name: "a" }, { name: "b" }]
                }
            ];
            let props = {
                isHost: true,
                activeRowIndex: 1,
                gridData: {
                    notificationTypes:data,
                    count: 1
                },
                gridParams: {
                    pageIndex: 1,
                    pageSize: 10
                },
                setGridParams: jest.fn(),
                getGridData: jest.fn()
            };

            // Act
            let dataGrid = (new ShallowRenderer()).render(<DataGrid {...props}/>);

            // Assert
            expect(dataGrid).toMatchSnapshot();
        });      
    });
});