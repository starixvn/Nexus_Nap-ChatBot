import React from 'react';
import { Header } from './../header';
import { shallow, mount, configure } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import renderer from 'react-test-renderer';

configure({ adapter: new Adapter() })

function setup(props, isShallow = true) {
    const wrap = isShallow ? (component) => shallow(component) : (component) => mount(component)
    return wrap(<Header {...props} />)
}

describe('Header', () => {
    describe('render', () => {
        it('sidebar is expanded', () => {
            // Arrange
            const props = {
                isSidebarExpanded: false
            }

            // Act
            const renderOutput = renderer.create(<Header {...props}/>).toJSON()

            // Assert
            expect(renderOutput).toMatchSnapshot()
        })

        it('sidebar is not expanded', () => {
            // Arrange
            const props = {
                isSidebarExpanded: true
            }

            // Act
            const renderOutput = renderer.create(<Header {...props}/>).toJSON()

            // Assert
            expect(renderOutput).toMatchSnapshot()
        })
    })

    describe('behavior', () => {
        it('should call collapseExpandHandler prop when click', () => {
            // Arrange
            const props = {
                collapseExpandHandler: jest.fn()
            }
            const sidebar = new setup(props)

            // Act
            sidebar.find('div.line-wrapp').simulate('click')

            // Assert
            expect(props.collapseExpandHandler).toHaveBeenCalled()
        })
    })
})
