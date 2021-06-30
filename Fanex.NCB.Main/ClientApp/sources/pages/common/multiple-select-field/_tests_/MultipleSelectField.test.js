import React from 'react';
import { MultipleSelectField } from '../MultipleSelectField';
import { shallow } from 'enzyme';

describe('MultipleSelectField', () => {
    it('when isSummitted and not valid - should match snapshot', () => {
        // Arrange
        let props = {
            isSummited: true
        };

        // Act
        let wrapper = shallow(
            <MultipleSelectField {...props} />
        );
        wrapper.instance().isValid = jest.fn(() => { return false; });
        wrapper.instance().forceUpdate();

        // Assert
        expect(wrapper.html()).toMatchSnapshot();
    });

    it('when isSummitted and valid - should match snapshot', () => {
        // Arrange
        let props = {
            isSummited: true
        };

        // Act
        let wrapper = shallow(
            <MultipleSelectField {...props} />
        );
        wrapper.instance().isValid = jest.fn(() => { return true; });
        wrapper.instance().forceUpdate();

        // Assert
        expect(wrapper.html()).toMatchSnapshot();
    });

    it('when not isSummitted - should match snapshot', () => {
        // Arrange
        let props = {
            isSummited: false
        };

        // Act
        let wrapper = shallow(
            <MultipleSelectField {...props} />
        );

        // Assert
        expect(wrapper.html()).toMatchSnapshot();
    });

    it('when not isSummitted and has data - should match snapshot', () => {
        // Arrange
        let props = {
            isSummited: false,
            data: [],
            width: 10
        };

        // Act
        let wrapper = shallow(
            <MultipleSelectField {...props} />
        );

        // Assert
        expect(wrapper.html()).toMatchSnapshot();
    });

    it('onChange - call onChange', () => {
        // Arrange
        let props = { onChange: jest.fn() };
        let inputField = new MultipleSelectField(props);
        inputField.isValid = jest.fn(() => { return true; });
        let value = "abc";

        // Act
        inputField.onChange(value);

        // Assert
        expect(props.onChange).toBeCalledWith({ value: "abc", isValid: true });
    });

    it('isValid - when value Not Empty - return true', () => {
        // Arrange
        let props = { onChange: jest.fn() };
        let inputField = new MultipleSelectField(props);
        let value = 'abc';

        // Act
        var result = inputField.isValid(value);

        // Assert
        expect(result).toBe(true);
    });

    it('isValid - when value Empty - return false', () => {
        // Arrange
        let props = { onChange: jest.fn() };
        let inputField = new MultipleSelectField(props);
        let value = '';

        // Act
        var result = inputField.isValid(value);

        // Assert
        expect(result).toBe(false);
    });
});