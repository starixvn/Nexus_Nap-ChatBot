import React from 'react';
import { Filter } from '../Filter';
import { shallow } from 'enzyme';

describe('Filter', () => {

    afterAll(() => {
        jest.clearAllMocks();
    });

    const initProps = {
        notificationTypes: [{ notificationTypeID: 1, notificationTypeName: 'test' }],
        setGridParams: jest.fn(),
        getGridData: jest.fn()
    };

    describe('componentDidMount', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {...initProps};

            // Act
            const container = shallow(<Filter {...props}/>);
            container.instance().componentDidMount();

            // Assert
            expect(props.setGridParams).toHaveBeenCalled();
        });
    });

    describe('handleNotificationTypeChange', () => {
        it('should setState exactly', () => {
            // Arrange
            const props = {...initProps};
            const mockEvent = {
                target: { value: 123 }
            };

            // Act
            const container = shallow(<Filter {...props}/>);
            container.instance().handleNotificationTypeChange(mockEvent);

            // Assert
            expect(container.state().notificationTypeId).toBe(123);
        });
    });

    describe('getParams', () => {
        it('should return exactly value', () => {
            // Arrange
            const props = {...initProps};
            const expected = {
                notificationTypeId: 1,
                pageIndex: 1,
                pageSize: 10
            };

            // Act
            const container = shallow(<Filter {...props}/>);
            container.setState({ notificationTypeId: 1 });
            const result = container.instance().getParams();

            // Assert
            expect(result).toEqual(expected);
        });
    });

    describe('submitHandler', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {...initProps};
            const mockEvent = {
                preventDefault: jest.fn()
            };

            // Act
            const container = shallow(<Filter {...props}/>);
            container.instance().submitHandler(mockEvent);

            // Assert
            expect(props.setGridParams).toHaveBeenCalled();
            expect(props.getGridData).toHaveBeenCalled();
        })
    });
});