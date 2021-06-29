import React from 'react';
import { TextField } from '../TextField';
import { shallow } from 'enzyme';

describe('TextField', () => {
    afterAll(() => {
        jest.resetAllMocks();
    });

    describe('render', () => {
        it('when valid - should match snapshot', () => {
            // Arrange
            const props = {};

            // Act
            const container = shallow(<TextField {...props} />);
            container.instance().isValid = jest.fn(() => true);
            container.instance().forceUpdate();

            // Assert
            expect(container.render()).toMatchSnapshot();
        });

        it('when invalid - should match snapshot', () => {
            // Arrange
            const props = {};

            // Act
            const container = shallow(<TextField {...props} />);
            container.instance().isValid = jest.fn(() => false);
            container.instance().forceUpdate();

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
            const container = shallow(<TextField {...props} />);
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
            const container = shallow(<TextField {...props} />);
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
            const container = shallow(<TextField {...props} />);
            container.setState({ value: 'abc' });

            // Act
            const result = container.instance().isValid();

            // Assert
            expect(result).toBeTruthy();
        });

        it('should return false when value is invalid', () => {
            // Arrange
            const props = { onChange: jest.fn() };
            const container = shallow(<TextField {...props} />);
            container.setState({ value: '' });

            // Act
            const result = container.instance().isValid();

            // Assert
            expect(result).toBeFalsy();
        });
    });
});