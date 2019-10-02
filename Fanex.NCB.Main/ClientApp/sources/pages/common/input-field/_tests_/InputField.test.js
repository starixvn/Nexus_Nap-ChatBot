import React from 'react';
import ShallowRenderer from 'react-test-renderer/shallow';
import { InputField } from '../InputField';
import { shallow, render, configure } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';

describe('InputField', () => {
    configure({ adapter: new Adapter() });

    it('when isSummitted and not valid - render correctly', () => {
        // Arrange
        let props = {
            isSummited: true
        };

        // Act
        let wrapper = shallow(
            <InputField {...props} />
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
            <InputField {...props} />
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
            <InputField {...props} />
        );       

        // Assert
        expect(wrapper.render()).toMatchSnapshot();
    });

    it('onChange - call setState', () => {
        // Arrange
        let props = {onChange: jest.fn()};
        let inputField = new InputField(props);
        inputField.setState = jest.fn();
        inputField.callOnChange = jest.fn();
        let event = {
            target: { value: "xx" }
        };

        // Act
        inputField.onChange(event);

        // Assert
        expect(inputField.setState).toBeCalledWith(
            { value: event.target.value },
            inputField.callOnChange);
    });

    it('callOnChange - call props.onChange', () => {
        // Arrange
        let props = { onChange: jest.fn() };
        let inputField = new InputField(props);
        inputField.state = { value: "ab" };
        inputField.isValid = jest.fn(() => { return true; });

        // Act
        inputField.callOnChange();

        // Assert
        expect(props.onChange).toBeCalledWith({ value: "ab", isValid: true });
    });

    it('isValid - when value Not Empty - return true', () => {
        // Arrange
        let props = { onChange: jest.fn() };
        let inputField = new InputField(props);
        inputField.state = { value: 'abc' };

        // Act
        var result = inputField.isValid();

        // Assert
        expect(result).toBe(true);
    });

    it('isValid - when value Empty - return false', () => {
        // Arrange
        let props = { onChange: jest.fn() };
        let inputField = new InputField(props);
        inputField.state = { value: '' };

        // Act
        var result = inputField.isValid();

        // Assert
        expect(result).toBe(false);
    });
});