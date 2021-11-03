import React from 'react';
import { Filter } from '../Filter';
import { shallow } from 'enzyme';
import { MultipleSelectField } from '../../../common/multiple-select-field/MultipleSelectField';
import FilterStore from '../Filter';
import configureMockStore from 'redux-mock-store';

const mockStore = configureMockStore();

describe('Filter', () => {
    afterAll(() => {
        jest.clearAllMocks();
    });

    const initProps = {
        selected: [
            { id: 1, name: 'Special Delete', checked: true, newMaxMessageId: 0 },
            { id: 2, name: 'Special Delete - Internal CSD', checked: true, newMaxMessageId: 0 },
            { id: 3, name: 'Site Message', checked: true, newMaxMessageId: 0 }
        ]
    };

    describe('handleChange', () => {
        it('should setState exactly when call function', () => {
            // Arrange
            const props = { ...initProps };
            const item = { value: '1, 2, 3' };
            const expected = [1, 2, 3];

            // Act
            const container = shallow(<Filter {...props}/>);
            container.instance().handleChange(item);

            // Assert
            expect(container.state().systemFilter).toEqual(expected);
        });
    });

    describe('handleSubmit', () => {
        it('should not call prop function when length of systemFilter is 0', () => {
            // Arrange
            const props = { ...initProps };
            const mockEvent = {
                preventDefault: jest.fn()
            };

            // Act
            const container = shallow(<Filter {...props}/>);
            container.instance().handleSubmit(mockEvent);
        });

        it('should call prop function when length of systemFilter greater than 0', () => {
            // Arrange
            const props = {
                ...initProps,
                setSelected: jest.fn()
            };
            const systemFilter = [2,3];
            const expected = [
                { id: 1, name: 'Special Delete', checked: false, newMaxMessageId: 0 },
                { id: 2, name: 'Special Delete - Internal CSD', checked: true, newMaxMessageId: 0 },
                { id: 3, name: 'Site Message', checked: true, newMaxMessageId: 0 }
            ]
            const mockEvent = {
                preventDefault: jest.fn()
            };

            // Act
            const container = shallow(<Filter {...props}/>);
            container.instance().setState({ systemFilter: systemFilter });
            container.instance().handleSubmit(mockEvent);

            //Assert
            expect(props.setSelected).toHaveBeenCalledWith(expected);
        });
    });

    describe('render', () => {
        it('should render MultipleSelectField exactly', () => {
            // Arrange
            const props = { ...initProps };
            const expected = { value: '1,2,3' };

            // Act
            const container = shallow(<Filter {...props}/>);
            container.instance().handleChange = jest.fn();
            container.find(MultipleSelectField).simulate('change', expected);

            // Assert
            expect(container.instance().handleChange).toHaveBeenCalledWith(expected);
        });

        it('props should be mapped correctly', () => {
            // Arrange
            const expected = {
                selected: [
                    { id: 1, name: 'Special Delete', checked: true, newMaxMessageId: 0 },
                    { id: 2, name: 'Special Delete - Internal CSD', checked: true, newMaxMessageId: 0 },
                    { id: 3, name: 'Site Message', checked: true, newMaxMessageId: 0 }
                ]
            };   
            const initialState = {
                selected: [
                    { id: 1, name: 'Special Delete', checked: true, newMaxMessageId: 0 },
                    { id: 2, name: 'Special Delete - Internal CSD', checked: true, newMaxMessageId: 0 },
                    { id: 3, name: 'Site Message', checked: true, newMaxMessageId: 0 }
                ]
            };

            const store = mockStore(initialState);

            // Act
            const container = shallow(<FilterStore store={store}/>);
            
            // Assert
            expect(container.props().selected).toEqual(expected.selected);        
        });
    });
});
