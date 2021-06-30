import React from 'react';
import { shallow } from 'enzyme';
import { InputField } from '../InputField';

describe('InputField', () => {
    afterAll(() => {
        jest.resetAllMocks();
    });

    describe('render', () => {
        it('when isSummitted and value is invalid - should match snapshot', () => {
            // Arrange
            const props = { isSummited: true };

            // Act
            const container = shallow(<InputField {...props} />);
            container.instance().isValid = jest.fn(() => false);
            container.instance().forceUpdate();

            // Assert
            expect(container.render()).toMatchSnapshot();
        });

        it('when isSummitted and value is valid - should match snapshot', () => {
            // Arrange
            const props = { isSummited: true };

            // Act
            const container = shallow(<InputField {...props} />);
            container.instance().isValid = jest.fn(() => true);
            container.instance().forceUpdate();

            // Assert
            expect(container.render()).toMatchSnapshot();
        });

        it('when not isSummitted - should match snapshot', () => {
            // Arrange
            const props = { isSummited: false };

            // Act
            const container = shallow(<InputField {...props} />);

            // Assert
            expect(container.render()).toMatchSnapshot();
        });
    });

    describe('onChange', () => {
        it('should update state', () => {
            // Arrange
            const props = { onChange: jest.fn() };
            const event = {
                target: { value: 'abc' }
            };
            const container = shallow(<InputField {...props} />);
            container.setState = jest.fn();
            container.instance().callOnChange = jest.fn();

            // Act
            container.instance().onChange(event);

            // Assert
            expect(container.setState).toBeCalledWith(
                { value: event.target.value },
                container.instance().callOnChange);
        });
    });

    describe('callOnChange', () => {
        it('should do as expected', () => {
            // Arrange
            const props = { onChange: jest.fn() };
            const container = shallow(<InputField {...props} />);
            container.setState({ value: 'abc' });
            container.instance().isValid = jest.fn(() => true);

            // Act
            container.instance().callOnChange();

            // Assert
            expect(props.onChange).toBeCalledWith({ value: 'abc', isValid: true });
        });
    });

    describe('isValid', () => {
        it('should return true when value is valid', () => {
            // Arrange
            const props = { onChange: jest.fn() };
            const container = shallow(<InputField {...props} />);
            container.setState({ value: 'abc' });

            // Act
            const result = container.instance().isValid();

            // Assert
            expect(result).toBeTruthy();
        });

        it('should return false when value is invalid', () => {
            // Arrange
            const props = { onChange: jest.fn() };
            const container = shallow(<InputField {...props} />);
            container.setState({ value: '' });

            // Act
            const result = container.instance().isValid();

            // Assert
            expect(result).toBeFalsy();
        });
    });
});