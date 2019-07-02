import React from 'react';
import Login  from '../login';
import { shallow, mount, configure } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';

configure({ adapter: new Adapter() })

function setup(props, isShallow = true) {
    const wrap = isShallow ? (component) => shallow(component) : (component) => mount(component)
    return wrap(<Login {...props} />)
}

describe('Login', () => {
    describe('state', () => {
        it('password is empty at first state', () => {
            // Arrange
            const props = {}

            // Act
            const wrapper = setup(props)

            // Assert
            expect(wrapper.state().password).toBe('')
        })

        it('message error is correct', () => {
            // Arrange
            const props = {}
            const wrapper = setup(props)

            // Act
            wrapper.setState({ invalidLogin: true })

            // Assert
            expect(wrapper.find('div.msg-error').text()).toBe('Invalid Username or Password')
        })
    })

    describe('behavior', () => {
        it('should call handleSubmit when submit form', () => {
            // Arrange
            const props = {}
            const wrapper = setup(props)
            wrapper.instance().handleSubmit = jest.fn()
            wrapper.instance().forceUpdate()

            // Act
            wrapper.find('form').simulate('submit')

            // Assert
            expect(wrapper.instance().handleSubmit).toHaveBeenCalled()
        })

        it('should call submitForm when handleSubmit is called', () => {
            // Arrange
            const props = {}
            const wrapper = setup(props, false)
            wrapper.instance().submitForm = jest.fn()
            wrapper.instance().forceUpdate()

            const mockEvent = {
                 preventDefault: jest.fn() 
            }

            // Act
            wrapper.instance().handleSubmit(mockEvent)

            // Assert
            expect(wrapper.instance().submitForm).toHaveBeenCalled()
        })
    })
})
