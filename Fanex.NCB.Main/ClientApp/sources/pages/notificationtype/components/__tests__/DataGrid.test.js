import React from 'react';
import { DataGrid } from '../DataGrid';
import { Cell } from '../../../common/fixed-table-header/index';
import { shallow } from 'enzyme';

describe('DataGrid', () => {

    afterAll(() => {
        jest.clearAllMocks();
    });

    const initProps = {
        gridData: [{
            notificationTypes:  [{
                name: 'test', recipientTypes: [{ name: 'UK' }], chatGroupTypes: [{ name: 'License' }],
                systemName: 'css' , unRemovable: true
            }],
            count: 10
        }],
        notificationTypes: [test. test, test],
        gridParams: { pageIndex: 1, pageSize: 100 },
        activeRowIndex: 1,
        setGridParams: jest.fn(),
        getGridData: jest.fn()
    };

    describe('componentWillMount', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {...initProps};

            // Act
            const container = shallow(<DataGrid {...props}/>);
            container.instance().componentWillMount();

            // Assert
            expect(props.setGridParams).toHaveBeenCalled();
            expect(props.getGridData).toHaveBeenCalled();
        });
    });

    describe('onPaging', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {...initProps};

            // Act
            const container = shallow(<DataGrid {...props}/>);
            container.instance().onPaging(1, 100);

            // Assert
            expect(props.setGridParams).toHaveBeenCalledWith({ pageIndex: 1, pageSize: 100});
            expect(props.getGridData).toHaveBeenCalledWith({ pageIndex: 1, pageSize: 100});
        });
    });

    describe('openEditPopup', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                setActiveRowIndex: jest.fn(),
                setCreateEditPopupData: jest.fn()
            };
            const item = {
                id: 1,
                name: 'test',
                recipientTypes: [{ id: 1 }, { id: 2 }],
                chatGroupTypes: [{ id: 1 }, { id: 2 }],
                systemId: 123
            };

            // Act
            const container = shallow(<DataGrid {...props}/>);
            container.instance().openEditPopup(item, 1);

            // Assert
            expect(props.setActiveRowIndex).toHaveBeenCalledWith(1);
            expect(props.setCreateEditPopupData).toHaveBeenCalled();
        });
    });

    describe('openDeletePopup', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                setActiveRowIndex: jest.fn(),
                setDeletePopupData: jest.fn()
            };

            // Act
            const container = shallow(<DataGrid {...props}/>);
            container.instance().openDeletePopup(1, 1, 'test');

            // Assert
            expect(props.setActiveRowIndex).toHaveBeenCalledWith(1);
            expect(props.setDeletePopupData).toHaveBeenCalledWith({
                isShowed: true,
                notificationTypeId: 1,
                content: 'test',
                successMsg: 'Delete Notification Type Successfully'
            });
        });
    });

    describe('buildHeader', () => {
        it('should render expected header when iHost is true', () => {
            // Arrange
            const props = {
                ...initProps,
                isHost: true
            };
            
            // Act
            const container = shallow(<DataGrid {...props}/>).instance();
            const header = shallow(container.buildHeader());

            // Assert
            expect(header.find(Cell).length).toBe(6);
        });

        it('should render expected header when iHost is false', () => {
            // Arrange
            const props = {
                ...initProps,
                isHost: false
            };
            
            // Act
            const container = shallow(<DataGrid {...props}/>).instance();
            const header = shallow(container.buildHeader());

            // Assert
            expect(header.find(Cell).length).toBe(5);
        });
    });

    describe('buildBody', () => {
        it('should render no data when data is empty and isHost is true', () => {
            // Arrange
            const props = {
                ...initProps,
                isHost: true
            };
            const data = [], pageIndex = 1, pageSize = 100, activeRowIndex = 1;

            // Act
            const container = shallow(<DataGrid {...props}/>);
            const actual = container.instance().buildBody(data, pageIndex, pageSize, activeRowIndex);

            // Assert
            expect(actual).toEqual([]);
        });

        it('should render expected rows when data is not empty', () => {
            // Arrange
            const props = {
                ...initProps,
                isHost: true
            };
            const data = [{
                name: 'test', recipientTypes: [{ name: 'UK' }], chatGroupTypes: [{ name: 'License' }],
                systemName: 'css' , unRemovable: true
            }];
            const pageIndex = 1, pageSize = 100, activeRowIndex = 1;

            // Act
            const container = shallow(<DataGrid {...props}/>).instance();
            const actual = container.buildBody(data, pageIndex, pageSize, activeRowIndex);

            // Assert
            expect(actual.length).toBe(1);
        });
    });

    describe('render', () => {
        it('should render null when gridData is null', () => {
            // Arrange
            const props = {
                ...initProps,
                gridData: null
            };

            // Act
            const container = shallow(<DataGrid {...props}/>);

            // Assert
            expect(container.type()).toEqual(null);
        });
    });
});