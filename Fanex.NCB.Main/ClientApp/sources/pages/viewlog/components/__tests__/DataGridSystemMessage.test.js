import React from 'react';
import { DataGridSystemMessage } from '../DataGridSystemMessage';
import { shallow } from 'enzyme';
import Labels from '../../../common/Labels';
import { Table, Row, Cell } from '../../../common/fixed-table-header/index';
import NoDataTitle from '../../../common/no-data-title/NoDataTitle';
import configureMockStore from 'redux-mock-store';
import DataGridSystemMessageContainer from '../DataGridSystemMessage';

const mockStore = configureMockStore();

describe('DataGridSystemMessage', () => {
    const props = {
        paramsPage: {
            pageIndex: 1,
            pageSize: 100
        },
        systemMessage: {
            total: 100
        }
    };
    afterAll(() => {
        jest.clearAllMocks();
    });

    describe('onPaging', () => {
        it('should call correctly functions', () => {
            // Arrange
            const propsFunc = {
                notificationId: 188,
                getSystemMessage: jest.fn(),
                getParamsPage: jest.fn(),
                paramsPage: {
                    pageIndex: 1,
                    pageSize: 100
                },
                systemMessage: {
                    total: 100
                }
            };
            const expectedParams = {notificationId: 188, pageIndex: 1, pageSize: 100 };
            

            // Act
            const container = shallow(<DataGridSystemMessage {...propsFunc} />);
            container.instance().onPaging(1, 100);

            // Assert
            expect(propsFunc.getSystemMessage).toHaveBeenCalledWith(expectedParams);
            expect(propsFunc.getParamsPage).toHaveBeenCalledWith(expectedParams);
        });
    });

    describe('buildBody', () => {
        it('should render no data when data is empty', () => {
            // Act
            const container = shallow(<DataGridSystemMessage {...props}/>);
            const actual = container.instance().buildBody(true);

            // Assert
            expect(actual).toEqual([<Row key={`body${1}`} ><Cell colSpan="5"><NoDataTitle /></Cell></Row >]);
        });

        it('should render expected rows when data is not empty', () => {
            // Arrange
            const propsData = {
                paramsPage: {
                    pageIndex: 1,
                    pageSize: 100
                },
                systemMessage: {
                    total: 100,
                    data:[{id:1234}]
                }
            };

            // Act
            const container = shallow(<DataGridSystemMessage {...propsData}/>).instance();
            container.renderRow = jest.fn(() => <div />);
            const actual = container.buildBody(true);

            // Assert
            expect(actual).toEqual([<div />]);
        });
    });

    describe('buildHeader', () => {
        const getHeader = function (isHost) {
            return (<Row >
                <Cell colWidth={10} header>{Labels.MessageID}</Cell>
                <Cell colWidth={10} header>{Labels.NotificationID}</Cell>
                <Cell colWidth={15} header>{Labels.CreatedDate}</Cell>
                <Cell colWidth={30} header>{Labels.ErrorMessage}</Cell>
            </Row>);
        };

        it('should render expected header when isHost = false', () => {
            // Arrange
            const expected = getHeader(false);
            
            // Act
            const container = shallow(<DataGridSystemMessage {...props}/>);
            const header = container.instance().buildHeader(false);

            // Assert
            expect(header).toEqual(expected);
        });

        it('should render expected header when isHost = true', () => {
            // Arrange
            const expected = getHeader(true);

            // Act
            const container = shallow(<DataGridSystemMessage {...props}/>);
            const header = container.instance().buildHeader(true);

            // Assert
            expect(header).toEqual(expected);
        });
    });

    describe('render', () => {
        it('should received props pageOption when props is not empty', () => {
            // Arrange
            const pagingOption = {
                PageIndex: props.paramsPage.pageIndex,
                PageSize: props.paramsPage.pageSize,
                TotalItem: props.systemMessage.total,
                PageList: [100, 200, 300, 500, 1000]
            };

            // Act
            const container = shallow(<DataGridSystemMessage {...props}/>);
            
            // Assert
            expect(container.find(Table).props().pageOption).toEqual(pagingOption);
        });

        it('props should be mapped correctly', () => {
            // Arrange
            const expected = {
                paramsPage: {
                    pageIndex: 1,
                    pageSize: 100
                },
                systemMessage: {
                    total: 100,
                }
            };   
            const initialState = {
                paramsPage: {
                    pageIndex: 1,
                    pageSize: 100
                },
                systemMessage: {
                    total: 100,
                }
            };
            const store = mockStore(initialState);

            // Act
            const container = shallow(<DataGridSystemMessageContainer {...props} store={store}/>);
            
            // Assert
            expect(container.props().paramsPage).toEqual(expected.paramsPage);
            expect(container.props().systemMessage).toEqual(expected.systemMessage);
        });
    });
})