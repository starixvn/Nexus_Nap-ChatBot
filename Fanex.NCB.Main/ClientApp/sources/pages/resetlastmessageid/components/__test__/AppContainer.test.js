import React from 'react';
import { shallow } from 'enzyme';
import { AppContainer } from '../AppContainer';
import { NCBBreadcrum } from '../../../common/breadcrumbs/breadcrumbs';
import AppContainerStore  from '../AppContainer';
import DataGrid from '../Datagrid';
import Filter from '../Filter';
import EditNewMessage from '../EditNewMessage';
import configureMockStore from 'redux-mock-store';

const mockStore = configureMockStore();

describe('AppContainer', () => {
    afterAll(() => {
        jest.clearAllMocks();
    });

    describe('render', () => {
        it('should render correctly default components', () => {
            // Arrange
            const props = {
                editNewMessagePopup: { show: true }
            }
            // Act
            const container = shallow(<AppContainer {...props}/>);

            // Assert
            expect(container).toMatchSnapshot();
            expect(container.find(NCBBreadcrum)).toHaveLength(1);
            expect(container.find(Filter)).toHaveLength(1);
            expect(container.find(DataGrid)).toHaveLength(1);
            expect(container.find(EditNewMessage)).toHaveLength(1);
        });

        it('should not render EditNewMessage', () => {
            // Arrange
            const props = {
                editNewMessagePopup: { show: false }
            }
            // Act
            const container = shallow(<AppContainer {...props}/>);

            // Assert
            expect(container.find(EditNewMessage)).toHaveLength(0);
        });

        it('should pass props exactly EditNewMessage', () => {
            // Arrange
            const props = {
                editNewMessagePopup: {
                    show: true,
                    jobIdentify: 1,
                    title: 'Special',
                    newMaxMessageId: 2
                }
            }
            // Act
            const container = shallow(<AppContainer {...props}/>);

            // Assert
            expect(container.find(EditNewMessage).props().id).toBe(1);
            expect(container.find(EditNewMessage).props().title).toBe('Special');
            expect(container.find(EditNewMessage).props().newMaxId).toBe(2);
        });

        it('props should be mapped correctly', () => {
            // Arrange
            const expected = {
                editNewMessagePopup: { show: false }
            };   
            const initialState = {
                editNewMessagePopup: { show: false }
            };

            const store = mockStore(initialState);

            // Act
            const container = shallow(<AppContainerStore store={store}/>);
            
            // Assert
            expect(container.props().editNewMessagePopup).toEqual(expected.editNewMessagePopup);        
        });
    });
});