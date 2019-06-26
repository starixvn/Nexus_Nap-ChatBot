import React from 'react';
import { App } from '../../App';
import { shallow, configure } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';

configure({ adapter: new Adapter() })

describe("App", () => {
    it("should render login child component with props as empty", () => {
        // Arrange
        const props = {}

        // Act
        const wrapper = shallow(<App/>)

        // Assert
        expect(wrapper.find('Login').props()).toEqual({})
    })
})