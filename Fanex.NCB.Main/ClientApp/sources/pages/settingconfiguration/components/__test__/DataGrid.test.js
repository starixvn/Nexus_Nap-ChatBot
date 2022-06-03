import React from 'react';
import { DataGrid } from '../DataGrid';
import { Row, Cell } from '../../../common/fixed-table-header/index';
import { shallow } from 'enzyme';
import NoDataTitle from '../../../common/no-data-title/NoDataTitle';

describe('DataGrid', () => {

    afterAll(() => {
        jest.clearAllMocks();
    });

    const initProps = {
        gridParams: { pageIndex: 1, pageSize: 100 },
        gridData: {
            data: [{ type: 'test', name: 'test123', unRemovable: true }]
        }
    };

    describe('onPaging', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                setGridParams: jest.fn(),
                getGridData: jest.fn()
            };

            const container = shallow(<DataGrid {...props}/>);

            // Act
            container.instance().onPaging(1, 100);

            // Assert
            expect(props.setGridParams).toHaveBeenCalled();
            expect(props.getGridData).toHaveBeenCalled();
        });
    });

    describe('buildHeader', () => {
        it('should render expected header when iHost is true', () => {
            // Arrange
            const props = { ...initProps };
            
            // Act
            const container = shallow(<DataGrid {...props}/>).instance();
            const header = shallow(container.buildHeader(true));

            // Assert
            expect(header.find(Cell).length).toBe(4);
        });

        it('should render expected header when iHost is false', () => {
            // Arrange
            const props = { ...initProps };
            
            // Act
            const container = shallow(<DataGrid {...props}/>).instance();
            const header = shallow(container.buildHeader(false));

            // Assert
            expect(header.find(Cell).length).toBe(3);
        });
    });

    describe('buildBody', () => {
        it('should render no data when data is empty and isHost is true', () => {
            // Arrange
            const props = {
                ...initProps,
                gridData: { data: [], total: 0 }
            };

            // Act
            const container = shallow(<DataGrid {...props}/>);
            const actual = container.instance().buildBody(true);

            // Assert
            expect(actual).toEqual([<Row key={`body${1}`} ><Cell colSpan="4"><NoDataTitle /></Cell></Row >]);
        });

        it('should render expected rows when data is not empty', () => {
            // Arrange
            const props = { ...initProps };

            // Act
            const container = shallow(<DataGrid {...props}/>).instance();
            container.renderRow = jest.fn(() => <div />);
            const actual = container.buildBody(true);

            // Assert
            expect(actual).toEqual([<div />]);
        });
    });

    describe('handleEditSettingType', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                getSettingType: jest.fn()
            };
            const mockEvent = {
                target: {
                    dataset: { type: 10 }
                }
            };

            // Act
            const container = shallow(<DataGrid {...props}/>);
            container.instance().handleEditSettingType(mockEvent);

            // Assert
            expect(props.getSettingType).toHaveBeenCalledWith(10);
        });
    });

    describe('handleDeleteSettingType', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                openDeleteSettingTypePopup: jest.fn()
            };
            const mockEvent = {
                target: {
                    dataset: { type: 10, name: 'test' }
                }
            };

            // Act
            const container = shallow(<DataGrid {...props}/>);
            container.instance().handleDeleteSettingType(mockEvent);

            // Assert
            expect(props.openDeleteSettingTypePopup).toHaveBeenCalledWith({ name: 'test', type: 10 });
        });
    });

    describe('renderRow', () => {
        it('should render correctly when unRemovable is true', () => {
            // Arrange
            const props = {...initProps};
            const data = { type: 'test', name: 'test123', unRemovable: true };

            // Act
            const container = shallow(<DataGrid {...props} />);
            container.instance().startIndex = 1;
            const renderRow = shallow(container.instance().renderRow(data, 1, true));

            // Assert
            expect(renderRow.find(Cell).length).toBe(4);
            expect(renderRow.find('span').at(1).props().className).toBe('icon-delete icon-disabled');
        });

        it('should render correctly when unRemovable is false', () => {
            // Arrange
            const props = {...initProps};
            const data = { type: 'test', name: 'test123', unRemovable: false };

            // Act
            const container = shallow(<DataGrid {...props} />);
            container.instance().startIndex = 1;
            const renderRow = shallow(container.instance().renderRow(data, 1, true));

            // Assert
            expect(renderRow.find(Cell).length).toBe(4);
            expect(renderRow.find('span').at(1).props().className).toBe('icon-delete');
        });

        it('should render correctly when isHost is false', () => {
            // Arrange
            const props = {...initProps};
            const data = { type: 'test', name: 'test123', unRemovable: false };

            // Act
            const container = shallow(<DataGrid {...props} />);
            container.instance().startIndex = 1;
            const renderRow = shallow(container.instance().renderRow(data, 1, false));

            // Assert
            expect(renderRow.find(Cell).length).toBe(3);
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