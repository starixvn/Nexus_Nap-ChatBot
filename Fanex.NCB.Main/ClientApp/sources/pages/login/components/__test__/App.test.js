import React from 'react';
import { App } from '../../App';
import { shallow, configure } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import Login from '../login';

configure({ adapter: new Adapter() })

describe('App', () => {
    it('should render login child component', () => {
        // Arrange
        const props = {}

        // Act
        const wrapper = shallow(<App/>)

        // Assert
        expect(wrapper.contains(<Login/>)).toBe(true)
    })
})