import React from 'react';
import { shallow } from 'enzyme';
import { AppContainer } from '../AppContainer';
import { NCBBreadcrum } from '../../../common/breadcrumbs/breadcrumbs';
import DataGridConversationMessage from '../DataGridConversationMessage';
import DataGridRecipientList from '../DataGridRecipientList';
import DataGridSystemMessage from '../DataGridSystemMessage';
import AppContainerStore  from '../AppContainer';
import configureMockStore from 'redux-mock-store';

const mockStore = configureMockStore();

describe('AppContainer', () => {
    afterAll(() => {
        jest.clearAllMocks();
    });

    describe('render', () => {
        it('should render setState showSystemMessage when Click', () => {
            // Arrange
            const expectStateShow = {
                showSystemMessage: true,
                showRecipientList: false,
                showConversation: false,
            };

            // Act
            const container = shallow(<AppContainer/>);
            container.instance().ShowOption('showSystemMessage');

            // Assert            
            expect(container.state('show')).toEqual(expectStateShow);
        });

        it('should render setState showRecipientList when Click', () => {
            // Arrange
            const expectStateShow = {
                showSystemMessage: false,
                showRecipientList: true,
                showConversation: false,
            };

            // Act
            const container = shallow(<AppContainer/>);
            container.instance().ShowOption('showRecipientList');

            // Assert            
            expect(container.state('show')).toEqual(expectStateShow);
        });

        it('should render setState showConversation when Click', () => {
            // Arrange
            const expectStateShow = {
                showSystemMessage: false,
                showRecipientList: false,
                showConversation: true,
            };

            // Act
            const container = shallow(<AppContainer/>);
            container.instance().ShowOption('showConversation');

            // Assert            
            expect(container.state('show')).toEqual(expectStateShow);
        });

        it('should render correctly default components', () => {
            // Act
            const container = shallow(<AppContainer/>);

            // Assert
            expect(container).toMatchSnapshot();
            expect(container.find(NCBBreadcrum)).toHaveLength(1);
            expect(container.find(DataGridConversationMessage)).toHaveLength(0);
            expect(container.find(DataGridRecipientList)).toHaveLength(0);
            expect(container.find(DataGridSystemMessage)).toHaveLength(1);
        });

        it('should render correctly RecipientList components', () => {
            // Arrange
            const state = {
                show:{
                    showSystemMessage: false,
                    showRecipientList: true,
                    showConversation: false,
                }
            }

            // Act
            const container = shallow(<AppContainer/>);
            container.setState(state);

            // Assert
            expect(container).toMatchSnapshot();
            expect(container.find(NCBBreadcrum)).toHaveLength(1);
            expect(container.find(DataGridConversationMessage)).toHaveLength(0);
            expect(container.find(DataGridRecipientList)).toHaveLength(1);
            expect(container.find(DataGridSystemMessage)).toHaveLength(0);
        });

        it('should render correctly ConversationMessage components', () => {
            // Arrange
            const state = {
                show:{
                    showSystemMessage: false,
                    showRecipientList: false,
                    showConversation: true,
                }
            }

            // Act
            const container = shallow(<AppContainer/>);
            container.setState(state);

            // Assert
            expect(container).toMatchSnapshot();
            expect(container.find(NCBBreadcrum)).toHaveLength(1);
            expect(container.find(DataGridConversationMessage)).toHaveLength(1);
            expect(container.find(DataGridRecipientList)).toHaveLength(0);
            expect(container.find(DataGridSystemMessage)).toHaveLength(0);
        });

        it('props should be mapped correctly', () => {
            // Arrange
            const expected = {
                paramsPageSystemMessage: {
                    pageIndex: 1,
                    pageSize: 100
                },
                paramsPageRecipientList: {
                    pageIndex: 1,
                    pageSize: 200
                },
                paramsPageConversationMessage: {
                    pageIndex: 1,
                    pageSize: 300
                },
            };   
            const initialState = {
                paramsPageSystemMessage: {
                    pageIndex: 1,
                    pageSize: 100
                },
                paramsPageRecipientList: {
                    pageIndex: 1,
                    pageSize: 200
                },
                paramsPageConversationMessage: {
                    pageIndex: 1,
                    pageSize: 300
                },
            };
            const store = mockStore(initialState);

            // Act
            const container = shallow(<AppContainerStore store={store}/>);
            
            // Assert
            expect(container.props().paramsPageSystemMessage).toEqual(expected.paramsPageSystemMessage);
            expect(container.props().paramsPageRecipientList).toEqual(expected.paramsPageRecipientList);
            expect(container.props().paramsPageConversationMessage).toEqual(expected.paramsPageConversationMessage);         
        });
    });
    
    describe('onClick', () => {
        it("should call ShowOption on click div choose-option", () => {
            // Act
            const container = shallow(<AppContainer />);
            const spy = jest.spyOn(container.instance(), "ShowOption");
            container.instance().forceUpdate();

            container.find(".choose-option").at(0).simulate("click");
            container.find(".choose-option").at(1).simulate("click");
            container.find(".choose-option").at(2).simulate("click");

            // Assert
            expect(spy).toHaveBeenCalled();
        });
    });

    describe('handleSubmit', () => {
        it('data is called - should call exactly functions when submit', () => {
            // Arrange
            let props = {
                paramsPage: {
                    pageIndex: 1,
                    pageSize: 100
                },
                getParamsPageSystemMessage: jest.fn(),
                getParamsPageRecipientList: jest.fn(),
                getParamsPageConversationMessage: jest.fn(),
                getNotificationSystemMessage: jest.fn(),
                getNotificationRecipientList: jest.fn(),
                getConversationMessageLog: jest.fn(),
            };
            const expectedParams = {notificationId: 188, pageIndex: 1, pageSize: 100 };
            const formEventMocked = { preventDefault: jest.fn() };
            
            //Act
            const container = shallow(<AppContainer {...props} />);
            container.setState({notificationId: 188});
            container.find("Form").simulate("submit", formEventMocked);

            // Assert
            expect(container.find("Form")).toHaveLength(1);
            expect(formEventMocked.preventDefault).toBeCalledTimes(1);
            expect(props.getParamsPageSystemMessage).toHaveBeenCalledWith(expectedParams);
            expect(props.getNotificationSystemMessage).toHaveBeenCalledWith(expectedParams);
            expect(props.getNotificationRecipientList).toHaveBeenCalledWith(expectedParams);
            expect(props.getConversationMessageLog).toHaveBeenCalledWith(expectedParams);
        });
    });

    describe('handleChangeNotificationId', () => {
        it("setState notificationId when type valid value", () => {
            // Arrange
            const value = "123";
            
            //Act
            const container = shallow(<AppContainer />);
            container.find("Input").simulate("change", { target: { value } });

            // Assert
            expect(container.find("Input")).toHaveLength(1);
            expect(container.state().notificationId).toBe('123');
        });

        it("setState notificationId when type invalid value", () => {
            // Arrange
            const value = "xyz";
            
            //Act
            const container = shallow(<AppContainer />);
            container.find("Input").simulate("change", { target: { value } });

            // Assert
            expect(container.state().notificationId).toBe('');
        });
    });
});