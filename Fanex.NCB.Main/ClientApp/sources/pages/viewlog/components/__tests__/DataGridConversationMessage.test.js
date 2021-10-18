import React from 'react';
import { DataGridConversationMessage } from '../DataGridConversationMessage';
import { shallow } from 'enzyme';
import Labels from '../../../common/Labels';
import { Table, Row, Cell } from '../../../common/fixed-table-header/index';
import NoDataTitle from '../../../common/no-data-title/NoDataTitle';
import configureMockStore from 'redux-mock-store';
import DataGridConversationMessageContainer from '../DataGridConversationMessage';
import PanelContent from '../../../common/panel-content/PanelContent';
import ContentPreview from '../../../common/ContentPreview';

const mockStore = configureMockStore();

describe('DataGridConversationMessage', () => {
    const props = {
        paramsPageConversationMessage: {
            pageIndex: 1,
            pageSize: 100
        },
        conversationMessages: {
            total: 0
        }
    };
    afterAll(() => {
        jest.clearAllMocks();
    });

    describe('toggleExpandPanels', () => {
        it('should set state when total conversationMessages < 0', () => {
            // Arrange
            const propsFunc = {
                paramsPageConversationMessage: {
                    pageIndex: 1,
                    pageSize: 100
                },
                conversationMessages: {
                    total: 0
                }
            };
            // Act
            const container = shallow(<DataGridConversationMessage {...propsFunc} />);
            container.instance().toggleExpandPanels();

            // Assert
            expect(container.state().expandPanels).toBe(true);
        });
    });
    
    describe('onPaging', () => {
        it('should call correctly functions', () => {
            // Arrange
            const propsFunc = {
                notificationId: 188,
                getConversationMessageLog: jest.fn(),
                getParamsPageConversationMessage: jest.fn(),
                paramsPageConversationMessage: {
                    pageIndex: 1,
                    pageSize: 100
                },
                conversationMessages: {
                    total: 0
                }
            };
            const expectedParams = {notificationId: 188, pageIndex: 1, pageSize: 100 };
            

            // Act
            const container = shallow(<DataGridConversationMessage {...propsFunc} />);
            container.instance().onPaging(1, 100);

            // Assert
            expect(propsFunc.getConversationMessageLog).toHaveBeenCalledWith(expectedParams);
            expect(propsFunc.getParamsPageConversationMessage).toHaveBeenCalledWith(expectedParams);
        });
    });

    describe('buildBody', () => {
        it('should render no data when data is empty', () => {
            // Act
            const container = shallow(<DataGridConversationMessage {...props}/>);
            const actual = container.instance().buildBody(true);

            // Assert
            expect(actual).toEqual([<Row key={`body${1}`} ><Cell colSpan="10"><NoDataTitle /></Cell></Row >]);
        });

        it('should render expected rows when data is not empty', () => {
            // Arrange
            const propsData = {
                paramsPageConversationMessage: {
                    pageIndex: 1,
                    pageSize: 100
                },
                conversationMessages: {
                    total: 100,
                    data:[{id:1234, content: 'test'}],
                    dataMapRecipient:[{id:1234}]
                }
            };

            // Act
            const container = shallow(<DataGridConversationMessage {...propsData}/>).instance();
            container.renderRow = jest.fn(() => <div />);
            const actual = container.buildBody(true);

            // Assert
            expect(actual).toEqual([<div />]);
        });
    });

    describe('buildHeader', () => {

        
        it('should render expected header', () => {
            // Arrange
            const state = { expandPanels: true };
            
            // Act
            const container = shallow(<DataGridConversationMessage {...props}/>).instance();
            container.setState(state);
            container.toggleExpandPanels = jest.fn();
            const header = shallow(container.buildHeader());
            header.find('span').at(0).simulate('click');

            // Assert
            expect(header.find(Cell).length).toBe(9);
            expect(container.toggleExpandPanels).toHaveBeenCalled();
        });
    });

    describe('renderRow', () => {
        it('PanelContent should be call', () => {
            // Arrange
            const propsFunc = {
                paramsPageConversationMessage: {
                    pageIndex: 1,
                    pageSize: 100
                },
                conversationMessages: {
                    total: 0
                }
            };
            const item = {
                content: `
                    test123456789123456789123456789123456789123456789123456798123
                    456789123456487976789123456789123456789123456789123456798123
                `
            };

            // Act
            const container = shallow(<DataGridConversationMessage {...propsFunc} />);
            const renderRow = shallow(container.instance().renderRow(item,1));

            // Assert
            expect(renderRow.find(PanelContent).length).toBe(1);
            expect(renderRow.find(PanelContent).props().content).toBe(item.content);
        });

        it('ContentPreview should be call', () => {
            // Arrange
            const propsFunc = {
                paramsPageConversationMessage: {
                    pageIndex: 1,
                    pageSize: 100
                },
                conversationMessages: {
                    total: 0
                }
            };
            const item = {
                content: 'test'
            };

            // Act
            const container = shallow(<DataGridConversationMessage {...propsFunc} />);
            const renderRow = shallow(container.instance().renderRow(item,1));

            // Assert
            expect(renderRow.find(ContentPreview).length).toBe(1);
            expect(renderRow.find(ContentPreview).props().text).toBe(item.content);
        });
    });

    describe('render', () => {
        it('should received props pageOption when props is not empty', () => {
            // Arrange
            const pagingOption = {
                PageIndex: props.paramsPageConversationMessage.pageIndex,
                PageSize: props.paramsPageConversationMessage.pageSize,
                TotalItem: props.conversationMessages.total,
                PageList: [100, 200, 300, 500, 1000]
            };

            // Act
            const container = shallow(<DataGridConversationMessage {...props}/>);
            
            // Assert
            expect(container.find(Table).props().pageOption).toEqual(pagingOption);
        });

        it('props should be mapped correctly', () => {
            // Arrange
            const expected = {
                paramsPageConversationMessage: {
                    pageIndex: 1,
                    pageSize: 100
                },
                conversationMessages: {
                    total: 0,
                }
            };   
            const initialState = {
                paramsPageConversationMessage: {
                    pageIndex: 1,
                    pageSize: 100
                },
                conversationMessages: {
                    total: 0,
                }
            };
            const store = mockStore(initialState);

            // Act
            const container = shallow(<DataGridConversationMessageContainer {...props} store={store}/>);
            
            // Assert
            expect(container.props().paramsPageConversationMessage).toEqual(expected.paramsPageConversationMessage);
            expect(container.props().conversationMessages).toEqual(expected.conversationMessages);
        });
    });
});