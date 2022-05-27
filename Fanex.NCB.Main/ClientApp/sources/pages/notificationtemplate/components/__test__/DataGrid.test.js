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
        gridData: {
            data: [{
                notificationTemplateId: 1, notificationTypeName: 'test', notificationTemplateName: 'test123',
                title: 'title', content: 'content', unRemovable: true
            }],
            total: 1
        },
        gridParams: {
            pageIndex: 1,
            pageSize: 100
        }
    };

    describe('toggleExpandPanels', () => {
        it('should setState exactly', () => {
            // Arrange
            const props = {...initProps};
            // Act
            const container = shallow(<DataGrid {...props} />);
            container.instance().toggleExpandPanels();

            // Assert
            expect(container.state().expandPanels).toBe(true);
        });
    });

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
            container.toggleExpandPanels = jest.fn();
            const header = shallow(container.buildHeader(true));
            header.find('span').simulate('click');

            // Assert
            expect(header.find(Cell).length).toBe(5);
            expect(container.toggleExpandPanels).toHaveBeenCalled();
        });

        it('should render expected header when iHost is false', () => {
            // Arrange
            const props = { ...initProps };
            
            // Act
            const container = shallow(<DataGrid {...props}/>).instance();
            const header = shallow(container.buildHeader(false));

            // Assert
            expect(header.find(Cell).length).toBe(4);
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
            expect(actual).toEqual([<Row key={`body${1}`} ><Cell colSpan="5"><NoDataTitle /></Cell></Row >]);
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

    describe('handleEditTemplate', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                getTemplate: jest.fn()
            };
            const mockEvent = {
                target: {
                    dataset: { templateid: 1 }
                }
            };

            // Act
            const container = shallow(<DataGrid {...props}/>);
            container.instance().handleEditTemplate(mockEvent);

            // Assert
            expect(props.getTemplate).toHaveBeenCalledWith(1);
        });
    });

    describe('handleDeleteTemplate', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                openDeleteTemplatePopup: jest.fn()
            };
            const mockEvent = {
                target: {
                    dataset: { templateid: 1 }
                }
            };

            // Act
            const container = shallow(<DataGrid {...props}/>);
            container.instance().handleDeleteTemplate(mockEvent);

            // Assert
            expect(props.openDeleteTemplatePopup).toHaveBeenCalledWith({templateId: 1});
        });
    });

    describe('renderRow', () => {
        it('should render correctly when unRemovable is true', () => {
            // Arrange
            const props = {...initProps};
            const data = {
                notificationTemplateId: 1, notificationTypeName: 'test', notificationTemplateName: 'test123',
                title: 'title', content: 'content', unRemovable: true
            };

            // Act
            const container = shallow(<DataGrid {...props} />);
            container.instance().startIndex = 1;
            const renderRow = shallow(container.instance().renderRow(data, 1, true));

            // Assert
            expect(renderRow.find(Cell).length).toBe(5);
            expect(renderRow.find('span').at(1).props().className).toBe('icon-delete icon-disabled');
        });

        it('should render correctly when unRemovable is false', () => {
            // Arrange
            const props = {...initProps};
            const data = {
                notificationTemplateId: 1, notificationTypeName: 'test', notificationTemplateName: 'test123',
                title: 'title', content: 'content', unRemovable: false
            };

            // Act
            const container = shallow(<DataGrid {...props} />);
            container.instance().startIndex = 1;
            const renderRow = shallow(container.instance().renderRow(data, 1, true));

            // Assert
            expect(renderRow.find(Cell).length).toBe(5);
            expect(renderRow.find('span').at(1).props().className).toBe('icon-delete');
            expect(renderRow.find('span').at(1).props()['data-templateid']).toBe(1);
        });

        it('should render correctly when isHost is false', () => {
            // Arrange
            const props = {...initProps};
            const data = {
                notificationTemplateId: 1, notificationTypeName: 'test', notificationTemplateName: 'test123',
                title: 'title', content: 'content', unRemovable: true
            };

            // Act
            const container = shallow(<DataGrid {...props} />);
            container.instance().startIndex = 1;
            const renderRow = shallow(container.instance().renderRow(data, 1, false));

            // Assert
            expect(renderRow.find(Cell).length).toBe(4);
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