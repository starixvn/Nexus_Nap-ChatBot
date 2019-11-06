import React from 'react';
import ShallowRenderer from 'react-test-renderer/shallow';
import { MultipleSelectField } from '../MultipleSelectField';
import { shallow, render, configure } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';

describe('MultipleSelectField', () => {
    configure({ adapter: new Adapter() });

    it('when isSummitted and not valid - render correctly', () => {
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
        expect(wrapper.render()).toMatchSnapshot();
    });

    it('when isSummitted and valid - render correctly', () => {
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
        expect(wrapper.render()).toMatchSnapshot();
    });

    it('when not isSummitted - render correctly', () => {
        // Arrange
        let props = {
            isSummited: false
        };

        // Act
        let wrapper = shallow(
            <MultipleSelectField {...props} />
        );

        // Assert
        expect(wrapper.render()).toMatchSnapshot();
    });

    it('when not isSummitted and has data - render correctly', () => {
        // Arrange
        let props = {
            isSummited: false,
            data: [
            ],
            width: 10
        };

        // Act
        let wrapper = shallow(
            <MultipleSelectField {...props} />
        );

        // Assert
        expect(wrapper.render()).toMatchSnapshot();
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