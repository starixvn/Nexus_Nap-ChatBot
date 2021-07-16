import React from 'react';
import { DataGridRecipientList } from '../DataGridRecipientList';
import { shallow } from 'enzyme';
import Labels from '../../../common/Labels';
import { Table, Row, Cell } from '../../../common/fixed-table-header/index';
import NoDataTitle from '../../../common/no-data-title/NoDataTitle';
import configureMockStore from 'redux-mock-store';
import DataGridRecipientListContainer from '../DataGridRecipientList';

const mockStore = configureMockStore();

describe('DataGridRecipientList', () => {
    const props = {
        paramsPage: {
            pageIndex: 1,
            pageSize: 100
        },
        recipientList: {
            total: 0,
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
                getRecipientList: jest.fn(),
                getParamsPage: jest.fn(),
                paramsPage: {
                    pageIndex: 1,
                    pageSize: 100
                },
                recipientList: {
                    total: 0
                }
            };
            const expectedParams = {notificationId: 188, pageIndex: 1, pageSize: 100 };
            

            // Act
            const container = shallow(<DataGridRecipientList {...propsFunc} />);
            container.instance().onPaging(1, 100);

            // Assert
            expect(propsFunc.getRecipientList).toHaveBeenCalledWith(expectedParams);
            expect(propsFunc.getParamsPage).toHaveBeenCalledWith(expectedParams);
        });
    });

    describe('buildBody', () => {
        it('should render no data when data is empty', () => {
            // Act
            const container = shallow(<DataGridRecipientList {...props}/>);
            const actual = container.instance().buildBody(true);

            // Assert
            expect(actual).toEqual([<Row key={`body${1}`} ><Cell colSpan="9"><NoDataTitle /></Cell></Row >]);
        });

        it('should render expected rows when data is not empty', () => {
            // Arrange
            const propsData = {
                paramsPage: {
                    pageIndex: 1,
                    pageSize: 100
                },
                recipientList: {
                    total: 100,
                    data:[{id:1234}]
                }
            };

            // Act
            const container = shallow(<DataGridRecipientList {...propsData}/>).instance();
            container.renderRow = jest.fn(() => <div />);
            const actual = container.buildBody(true);

            // Assert
            expect(actual).toEqual([<div />]);
        });
    });

    describe('buildHeader', () => {
        const getHeader = function (isHost) {
            return (<Row >
                <Cell colWidth={10} header>{Labels.NotificationID}</Cell>
                <Cell colWidth={30} header>{Labels.GroupName}</Cell>
                <Cell colWidth={10} header>{Labels.ChatGroupType}</Cell>
                <Cell colWidth={10} header>{Labels.Recipient}</Cell>
                <Cell colWidth={10} header>{Labels.SentTime}</Cell>
                <Cell colWidth={10} header>{Labels.Status}</Cell>
                <Cell colWidth={10} header>{Labels.RetryTime}</Cell>
                <Cell colWidth={30} header>{Labels.ErrorMessage}</Cell>
            </Row>);
        };

        it('should render expected header when isHost = false', () => {
            // Arrange
            const expected = getHeader(false);

            // Act
            const container = shallow(<DataGridRecipientList {...props}/>);
            const header = container.instance().buildHeader(false);

            // Assert
            expect(header).toEqual(expected);
        });

        it('should render expected header when isHost = true', () => {
            // Arrange
            const expected = getHeader(true);

            // Act
            const container = shallow(<DataGridRecipientList {...props}/>);
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
                TotalItem: props.recipientList.total,
                PageList: [100, 200, 300, 500, 1000]
            };

            // Act
            const container = shallow(<DataGridRecipientList {...props}/>);
            
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
                recipientList: {
                    total: 0,
                }
            };    
            const initialState = {
                paramsPage: {
                    pageIndex: 1,
                    pageSize: 100
                },
                recipientList: {
                    total: 0,
                }
            };
            const store = mockStore(initialState);

            // Act
            const container = shallow(<DataGridRecipientListContainer {...props} store={store}/>);
            
            // Assert
            expect(container.props().paramsPage).toEqual(expected.paramsPage);
            expect(container.props().recipientList).toEqual(expected.recipientList);
        });
    });
});