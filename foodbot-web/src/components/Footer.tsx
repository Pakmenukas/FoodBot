import packageJson from '../../package.json';

export default function Footer() {
    return (
        <footer className="footer footer-center bg-base-300/60 p-4">
            <aside>
                <p>© {new Date().getFullYear()} FoodFortress v {packageJson.version}</p>
            </aside>
        </footer>
    )
}