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
        gridParams: {
            pageIndex: 1,
            pageSize: 100
        },
        gridData: {
            data: [
                { 
                    notificationTypeName: 'License', notificationTemplateName: 'test', systemName: 'UDO',
                    recipientTypeNames: 'testing', msgContentsName: 'test132', requiredParams: '1,2,3', isTest: true,
                    notificationTemplateID: 12, msgContentsType: 123, unRemovable: true
                }
            ]
        }
    };

    describe('onPaging', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
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
            // Act
            const container = shallow(<DataGrid/>).instance();
            const header = shallow(container.buildHeader(true));

            // Assert
            expect(header.find(Cell).length).toBe(9);
        });

        it('should render expected header when iHost is false', () => {
            // Act
            const container = shallow(<DataGrid/>).instance();
            const header = shallow(container.buildHeader(false));

            // Assert
            expect(header.find(Cell).length).toBe(8);
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
            const actual = container.instance().buildBody(true, true);

            // Assert
            expect(actual).toEqual([<Row key={`body${1}`} ><Cell colSpan="9"><NoDataTitle /></Cell></Row >]);
        });

        it('should render expected rows when data is not empty', () => {
            // Arrange
            const props = { ...initProps };

            // Act
            const container = shallow(<DataGrid {...props}/>).instance();
            container.renderRow = jest.fn(() => <div />);
            const actual = container.buildBody(true, true);

            // Assert
            expect(actual).toEqual([<div />]);
        });
    });

    describe('handleEditMapping', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                jobInfo: { id: 1 },
                setNotificationTypeRef: jest.fn(),
                getMapping: jest.fn()
            };
            const mockEvent = {
                target: {
                    dataset: { msgcontentstype: 1 }
                }
            };

            // Act
            const container = shallow(<DataGrid {...props}/>);
            container.instance().handleEditMapping(mockEvent);

            // Assert
            expect(props.getMapping).toHaveBeenCalled();
        });
    });

    describe('handleDeleteMapping', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                openDeleteMappingPopup: jest.fn()
            };
            const mockEvent = {
                target: {
                    dataset: {
                        templateid: 1,
                        msgcontentstype: 1,
                        typename: 'test',
                        templatename: 'testing',

                    }
                }
            };

            // Act
            const container = shallow(<DataGrid {...props}/>);
            container.instance().handleDeleteMapping(mockEvent);

            // Assert
            expect(props.openDeleteMappingPopup).toHaveBeenCalledWith({ msgContentsType: 1, notificationTemplateName: 'testing', notificationTypeName: 'test', templateId: 1 });
        });
    });

    describe('handleRequiredParams', () => {
        it('should return exactly result', () => {
            // Arrange
            const value = '1,2,3,4';

            // Act
            const container = shallow(<DataGrid />);
            const actual = container.instance().handleRequiredParams(value);

            // Assert
            expect(actual).toEqual('1, 2, 3, 4');
        });
    });

    describe('renderRow', () => {
        it('should render correctly when isIntelnalCSDJob is true', () => {
            // Arrange
            const props = {...initProps};
            const data = {
                notificationTemplateId: 1, notificationTypeName: 'test', notificationTemplateName: 'test123',
                title: 'title', content: 'content', unRemovable: true, systemName: 'sys', recipientTypeNames: 'recipient',
                msgContentsName: 'content', requiredParams: '1,2', isTest: true
            };

            // Act
            const container = shallow(<DataGrid {...props} />);
            container.instance().startIndex = 1;
            const renderRow = shallow(container.instance().renderRow(data, 1, true, true));

            // Assert
            expect(renderRow.find(Cell).length).toBe(9);
            expect(renderRow.find('span').length).toBe(3);
            expect(renderRow.find('span').at(0).props().className).toBe('icon-check');
            expect(renderRow.find('span').at(1).props().className).toBe('icon-edit icon-disabled');
            expect(renderRow.find('span').at(2).props().className).toBe('icon-delete icon-disabled');
        });

        it('should render correctly when isIntelnalCSDJob is false', () => {
            // Arrange
            const props = {...initProps};
            const data = {
                notificationTemplateId: 1, notificationTypeName: 'test', notificationTemplateName: 'test123',
                title: 'title', content: 'content', unRemovable: false, systemName: 'sys', recipientTypeNames: 'recipient',
                msgContentsName: 'content', requiredParams: '1,2', isTest: false
            };

            // Act
            const container = shallow(<DataGrid {...props} />);
            container.instance().startIndex = 1;
            const renderRow = shallow(container.instance().renderRow(data, 1, true, false));

            // Assert
            expect(renderRow.find(Cell).length).toBe(9);
            expect(renderRow.find('span').length).toBe(2);
            expect(renderRow.find('span').at(0).props().className).toBe('icon-edit');
            expect(renderRow.find('span').at(1).props().className).toBe('icon-delete');
        });

        it('should render correctly when isHost is false', () => {
            // Arrange
            const props = {...initProps};
            const data = {
                notificationTemplateId: 1, notificationTypeName: 'test', notificationTemplateName: 'test123',
                title: 'title', content: 'content', unRemovable: false, systemName: 'sys', recipientTypeNames: 'recipient',
                msgContentsName: 'content', requiredParams: '1,2', isTest: false
            };

            // Act
            const container = shallow(<DataGrid {...props} />);
            container.instance().startIndex = 1;
            const renderRow = shallow(container.instance().renderRow(data, 1, false, false));

            // Assert
            expect(renderRow.find(Cell).length).toBe(8);
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